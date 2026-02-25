using NChat.Web.Application.Abstractions;
using NChat.Web.Application.Dto;
using NChat.Web.Domain.Entities;
using NChat.Web.Domain.Exceptions;
using NChat.Web.Domain.ValueObjects;

namespace NChat.Web.Application.Services;

public sealed class ChatApplicationService
{
    private readonly IUserSessionRepository _sessions;
    private readonly IChatRoomRepository _rooms;
    private readonly IChatMessageRepository _messages;
    private readonly IUnitOfWork _unitOfWork;

    public ChatApplicationService(
        IUserSessionRepository sessions,
        IChatRoomRepository rooms,
        IChatMessageRepository messages,
        IUnitOfWork unitOfWork)
    {
        _sessions = sessions;
        _rooms = rooms;
        _messages = messages;
        _unitOfWork = unitOfWork;
    }

    public async Task RegisterConnectionAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        var existing = await _sessions.GetByConnectionIdAsync(connectionId, cancellationToken);
        if (existing is null)
        {
            existing = new UserSession(connectionId);
        }

        await _sessions.UpsertAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkDisconnectedAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        var existing = await _sessions.GetByConnectionIdAsync(connectionId, cancellationToken);
        if (existing is null)
        {
            return;
        }

        existing.MarkDisconnected();
        await _sessions.UpsertAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid?> GetCurrentRoomIdAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        var session = await _sessions.GetByConnectionIdAsync(connectionId, cancellationToken);
        return session?.CurrentRoomId;
    }

    public async Task<ClaimUsernameResult> ClaimUsernameAsync(string connectionId, string usernameInput, CancellationToken cancellationToken = default)
    {
        var username = Username.Create(usernameInput);
        var conflict = await _sessions.GetActiveByUsernameAsync(username.Value, cancellationToken);
        if (conflict is not null && !string.Equals(conflict.ConnectionId, connectionId, StringComparison.Ordinal))
        {
            return new ClaimUsernameResult(false, null, "Username is already in use.");
        }

        var session = await _sessions.GetByConnectionIdAsync(connectionId, cancellationToken) ?? new UserSession(connectionId);
        session.ClaimUsername(username);

        await _sessions.UpsertAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClaimUsernameResult(true, username.Value, null);
    }

    public async Task<RoomSummaryDto> CreateRoomAsync(string nameInput, string createdByUsername, CancellationToken cancellationToken = default)
    {
        var roomName = RoomName.Create(nameInput);
        if (await _rooms.ExistsByNameAsync(roomName.Value, cancellationToken))
        {
            throw new DomainValidationException("Room name already exists.");
        }

        var room = new ChatRoom(roomName, createdByUsername);
        await _rooms.AddAsync(room, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RoomSummaryDto(room.Id, room.NameValue, 0);
    }

    public async Task<List<RoomSummaryDto>> ListRoomsAsync(CancellationToken cancellationToken = default)
    {
        var rooms = await _rooms.ListAsync(cancellationToken);
        var activeSessions = await _sessions.ListActiveAsync(cancellationToken);

        var counts = activeSessions
            .Where(x => x.CurrentRoomId.HasValue)
            .GroupBy(x => x.CurrentRoomId!.Value)
            .ToDictionary(x => x.Key, x => x.Count());

        return rooms
            .OrderBy(x => x.NameValue)
            .Select(x => new RoomSummaryDto(x.Id, x.NameValue, counts.GetValueOrDefault(x.Id, 0)))
            .ToList();
    }

    public async Task JoinRoomAsync(string connectionId, Guid roomId, CancellationToken cancellationToken = default)
    {
        var room = await _rooms.GetByIdAsync(roomId, cancellationToken);
        if (room is null)
        {
            throw new DomainValidationException("Room does not exist.");
        }

        var session = await _sessions.GetByConnectionIdAsync(connectionId, cancellationToken)
            ?? throw new DomainValidationException("Session was not found.");

        session.JoinRoom(roomId);
        await _sessions.UpsertAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task LeaveCurrentRoomAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        var session = await _sessions.GetByConnectionIdAsync(connectionId, cancellationToken);
        if (session is null)
        {
            return;
        }

        session.LeaveRoom();
        await _sessions.UpsertAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<MessageDto> SendMessageAsync(string connectionId, string contentInput, CancellationToken cancellationToken = default)
    {
        var session = await _sessions.GetByConnectionIdAsync(connectionId, cancellationToken)
            ?? throw new DomainValidationException("Session was not found.");

        if (session.CurrentRoomId is null)
        {
            throw new DomainValidationException("Join a room before sending messages.");
        }

        if (string.IsNullOrWhiteSpace(session.UsernameValue))
        {
            throw new DomainValidationException("Set username before sending messages.");
        }

        var message = ChatMessage.Create(
            session.CurrentRoomId.Value,
            Username.Create(session.UsernameValue),
            MessageContent.Create(contentInput));

        await _messages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MessageDto(message.Id, message.RoomId, message.Username, message.Content, message.SentAt);
    }

    public async Task<List<PresenceUserDto>> GetPresenceAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var activeSessions = await _sessions.ListActiveAsync(cancellationToken);

        return activeSessions
            .Where(x => x.CurrentRoomId == roomId && !string.IsNullOrWhiteSpace(x.UsernameValue))
            .Select(x => new PresenceUserDto(x.UsernameValue!, x.ConnectedAt))
            .OrderBy(x => x.Username)
            .ToList();
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid roomId, int take = 100, CancellationToken cancellationToken = default)
    {
        var boundedTake = Math.Clamp(take, 1, 100);
        var messages = await _messages.ListRecentByRoomAsync(roomId, boundedTake, cancellationToken);

        return messages
            .OrderBy(x => x.SentAt)
            .Select(x => new MessageDto(x.Id, x.RoomId, x.Username, x.Content, x.SentAt))
            .ToList();
    }
}
