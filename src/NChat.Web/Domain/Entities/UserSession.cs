using NChat.Web.Domain.Exceptions;
using NChat.Web.Domain.ValueObjects;

namespace NChat.Web.Domain.Entities;

public sealed class UserSession
{
    private UserSession()
    {
    }

    public UserSession(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            throw new DomainValidationException("Connection id is required.");
        }

        Id = Guid.NewGuid();
        ConnectionId = connectionId;
        IsActive = true;
        ConnectedAt = DateTimeOffset.UtcNow;
        LastSeenAt = ConnectedAt;
    }

    public Guid Id { get; private set; }

    public string ConnectionId { get; private set; } = string.Empty;

    public string? UsernameValue { get; private set; }

    public Guid? CurrentRoomId { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset ConnectedAt { get; private set; }

    public DateTimeOffset LastSeenAt { get; private set; }

    public void ClaimUsername(Username username)
    {
        UsernameValue = username.Value;
        LastSeenAt = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    public void JoinRoom(Guid roomId)
    {
        if (!IsActive)
        {
            throw new DomainValidationException("Inactive session cannot join rooms.");
        }

        if (UsernameValue is null)
        {
            throw new DomainValidationException("Username must be claimed before joining rooms.");
        }

        CurrentRoomId = roomId;
        LastSeenAt = DateTimeOffset.UtcNow;
    }

    public void LeaveRoom()
    {
        CurrentRoomId = null;
        LastSeenAt = DateTimeOffset.UtcNow;
    }

    public void MarkDisconnected()
    {
        IsActive = false;
        CurrentRoomId = null;
        LastSeenAt = DateTimeOffset.UtcNow;
    }
}
