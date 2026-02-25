using Microsoft.AspNetCore.SignalR;
using NChat.Web.Application.Dto;
using NChat.Web.Application.Services;
using NChat.Web.Domain.Exceptions;

namespace NChat.Web.Presentation.Hubs;

public sealed class ChatHub : Hub
{
    private readonly ChatApplicationService _chatService;

    public ChatHub(ChatApplicationService chatService)
    {
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        await _chatService.RegisterConnectionAsync(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var oldRoomId = await GetCurrentRoomId();
        await _chatService.MarkDisconnectedAsync(Context.ConnectionId);
        if (oldRoomId.HasValue)
        {
            await BroadcastPresenceChanged(oldRoomId.Value);
        }
        await BroadcastRoomsChanged();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SetUsername(string username)
    {
        var result = await _chatService.ClaimUsernameAsync(Context.ConnectionId, username);
        await Clients.Caller.SendAsync("NameClaimResult", result.Ok, result.Reason);
        if (result.Ok)
        {
            await BroadcastRoomsChanged();
        }
    }

    public async Task JoinRoom(Guid roomId)
    {
        var oldRoomId = await GetCurrentRoomId();

        await _chatService.JoinRoomAsync(Context.ConnectionId, roomId);

        if (oldRoomId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, RoomGroup(oldRoomId.Value));
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(roomId));
        await BroadcastPresenceChanged(roomId);

        if (oldRoomId.HasValue && oldRoomId.Value != roomId)
        {
            await BroadcastPresenceChanged(oldRoomId.Value);
        }

        await BroadcastRoomsChanged();
    }

    public async Task LeaveCurrentRoom()
    {
        var oldRoomId = await GetCurrentRoomId();
        await _chatService.LeaveCurrentRoomAsync(Context.ConnectionId);

        if (oldRoomId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, RoomGroup(oldRoomId.Value));
            await BroadcastPresenceChanged(oldRoomId.Value);
        }

        await BroadcastRoomsChanged();
    }

    public async Task SendMessage(string content)
    {
        try
        {
            var message = await _chatService.SendMessageAsync(Context.ConnectionId, content);
            await Clients.Group(RoomGroup(message.RoomId)).SendAsync("RoomMessagePosted", message);
        }
        catch (DomainValidationException ex)
        {
            await Clients.Caller.SendAsync("HubError", ex.Message);
        }
    }

    public async Task GetCurrentPresenceSnapshot(Guid roomId)
    {
        await BroadcastPresenceChanged(roomId);
    }

    private async Task<Guid?> GetCurrentRoomId()
    {
        return await _chatService.GetCurrentRoomIdAsync(Context.ConnectionId);
    }

    private async Task BroadcastPresenceChanged(Guid roomId)
    {
        var presence = await _chatService.GetPresenceAsync(roomId);
        await Clients.Group(RoomGroup(roomId)).SendAsync("RoomPresenceChanged", roomId, presence);
    }

    private async Task BroadcastRoomsChanged()
    {
        var rooms = await _chatService.ListRoomsAsync();
        await Clients.All.SendAsync("RoomsChanged", rooms);
    }

    private static string RoomGroup(Guid roomId) => $"room:{roomId}";
}
