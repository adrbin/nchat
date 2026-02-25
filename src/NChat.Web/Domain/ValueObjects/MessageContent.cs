using NChat.Web.Domain.Exceptions;

namespace NChat.Web.Domain.ValueObjects;

public sealed record MessageContent
{
    public string Value { get; }

    private MessageContent(string value)
    {
        Value = value;
    }

    public static MessageContent Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new DomainValidationException("Message cannot be empty.");
        }

        var normalized = input.Trim();
        if (normalized.Length > 2000)
        {
            throw new DomainValidationException("Message cannot exceed 2000 characters.");
        }

        return new MessageContent(normalized);
    }
}
