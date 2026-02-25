using FluentAssertions;
using NChat.Web.Application.Abstractions;
using NChat.Web.Application.Services;
using NChat.Web.Domain.Entities;
using NChat.Web.Domain.Exceptions;
using NChat.Web.Domain.ValueObjects;

namespace NChat.Application.Tests;

public sealed class ChatApplicationServiceTests
{
    [Fact]
    public async Task ClaimUsername_Should_Fail_When_Already_Used_By_Another_Connection()
    {
        var fixture = new Fixture();
        var existing = new UserSession("conn-1");
        existing.ClaimUsername(Username.Create("john"));
        await fixture.Sessions.UpsertAsync(existing);

        var result = await fixture.Service.ClaimUsernameAsync("conn-2", "john");

        result.Ok.Should().BeFalse();
    }

    [Fact]
    public async Task CreateRoom_Should_Reject_Duplicate_CaseInsensitive_Name()
    {
        var fixture = new Fixture();
        await fixture.Service.CreateRoomAsync("General", "john");

        var act = async () => await fixture.Service.CreateRoomAsync("general", "jane");
        await act.Should().ThrowAsync<DomainValidationException>();
    }

    [Fact]
    public async Task SendMessage_Should_Reject_When_User_Not_In_Room()
    {
        var fixture = new Fixture();
        await fixture.Service.RegisterConnectionAsync("conn-1");
        await fixture.Service.ClaimUsernameAsync("conn-1", "john");

        var act = async () => await fixture.Service.SendMessageAsync("conn-1", "hello");
        await act.Should().ThrowAsync<DomainValidationException>();
    }

    private sealed class Fixture
    {
        public InMemoryUserSessionRepository Sessions { get; } = new();

        private InMemoryChatRoomRepository Rooms { get; } = new();

        private InMemoryChatMessageRepository Messages { get; } = new();

        private InMemoryUnitOfWork UnitOfWork { get; } = new();

        public ChatApplicationService Service { get; }

        public Fixture()
        {
            Service = new ChatApplicationService(Sessions, Rooms, Messages, UnitOfWork);
        }
    }

    private sealed class InMemoryUserSessionRepository : IUserSessionRepository
    {
        private readonly List<UserSession> _items = [];

        public Task<UserSession?> GetByConnectionIdAsync(string connectionId, CancellationToken cancellationToken = default)
            => Task.FromResult(_items.SingleOrDefault(x => x.ConnectionId == connectionId));

        public Task<UserSession?> GetActiveByUsernameAsync(string username, CancellationToken cancellationToken = default)
            => Task.FromResult(_items.SingleOrDefault(
                x => x.IsActive && x.UsernameValue != null && string.Equals(x.UsernameValue, username, StringComparison.OrdinalIgnoreCase)));

        public Task<List<UserSession>> ListActiveAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_items.Where(x => x.IsActive).ToList());

        public Task UpsertAsync(UserSession session, CancellationToken cancellationToken = default)
        {
            if (_items.All(x => x.Id != session.Id))
            {
                _items.Add(session);
            }

            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryChatRoomRepository : IChatRoomRepository
    {
        private readonly List<ChatRoom> _items = [];

        public Task<ChatRoom?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
            => Task.FromResult(_items.SingleOrDefault(x => x.Id == roomId));

        public Task<bool> ExistsByNameAsync(string roomName, CancellationToken cancellationToken = default)
            => Task.FromResult(_items.Any(x => string.Equals(x.NameValue, roomName, StringComparison.OrdinalIgnoreCase)));

        public Task AddAsync(ChatRoom room, CancellationToken cancellationToken = default)
        {
            _items.Add(room);
            return Task.CompletedTask;
        }

        public Task<List<ChatRoom>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_items.ToList());
    }

    private sealed class InMemoryChatMessageRepository : IChatMessageRepository
    {
        private readonly List<ChatMessage> _items = [];

        public Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            _items.Add(message);
            return Task.CompletedTask;
        }

        public Task<List<ChatMessage>> ListRecentByRoomAsync(Guid roomId, int take, CancellationToken cancellationToken = default)
            => Task.FromResult(
                _items.Where(x => x.RoomId == roomId)
                    .OrderByDescending(x => x.SentAt)
                    .Take(take)
                    .ToList());
    }

    private sealed class InMemoryUnitOfWork : IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
