using NChat.Web.Domain.Exceptions;
using NChat.Web.Domain.ValueObjects;

namespace NChat.Web.Domain.Entities;

public sealed class ChatMessage
{
    private ChatMessage()
    {
    }

    private ChatMessage(Guid roomId, Username username, MessageContent content)
    {
        Id = Guid.NewGuid();
        RoomId = roomId;
        Username = username.Value;
        Content = content.Value;
        SentAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid RoomId { get; private set; }

    public string Username { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public DateTimeOffset SentAt { get; private set; }

    public static ChatMessage Create(Guid roomId, Username username, MessageContent content)
    {
        if (roomId == Guid.Empty)
        {
            throw new DomainValidationException("Room id is required.");
        }

        return new ChatMessage(roomId, username, content);
    }
}
