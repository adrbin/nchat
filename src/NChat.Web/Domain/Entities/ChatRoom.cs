using NChat.Web.Domain.ValueObjects;

namespace NChat.Web.Domain.Entities;

public sealed class ChatRoom
{
    private ChatRoom()
    {
    }

    public ChatRoom(RoomName name, string createdByUsername)
    {
        Id = Guid.NewGuid();
        NameValue = name.Value;
        CreatedByUsername = createdByUsername;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string NameValue { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public string CreatedByUsername { get; private set; } = string.Empty;

    public void Rename(RoomName newName)
    {
        NameValue = newName.Value;
    }
}
