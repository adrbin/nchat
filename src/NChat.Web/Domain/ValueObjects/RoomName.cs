using NChat.Web.Domain.Exceptions;

namespace NChat.Web.Domain.ValueObjects;

public sealed record RoomName
{
    public string Value { get; }

    private RoomName(string value)
    {
        Value = value;
    }

    public static RoomName Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new DomainValidationException("Room name is required.");
        }

        var normalized = input.Trim();
        if (normalized.Length is < 2 or > 40)
        {
            throw new DomainValidationException("Room name length must be between 2 and 40.");
        }

        return new RoomName(normalized);
    }
}
