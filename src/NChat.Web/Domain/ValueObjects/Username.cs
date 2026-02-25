using System.Text.RegularExpressions;
using NChat.Web.Domain.Exceptions;

namespace NChat.Web.Domain.ValueObjects;

public sealed partial record Username
{
    private static readonly Regex AllowedPattern = AllowedRegex();

    public string Value { get; }

    private Username(string value)
    {
        Value = value;
    }

    public static Username Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new DomainValidationException("Username is required.");
        }

        var normalized = input.Trim();
        if (normalized.Length is < 3 or > 24)
        {
            throw new DomainValidationException("Username length must be between 3 and 24.");
        }

        if (!AllowedPattern.IsMatch(normalized))
        {
            throw new DomainValidationException("Username can contain letters, numbers, '_' and '-'.");
        }

        return new Username(normalized);
    }

    public bool EqualsNormalized(string other) =>
        string.Equals(Value, other, StringComparison.OrdinalIgnoreCase);

    [GeneratedRegex("^[A-Za-z0-9_-]+$")]
    private static partial Regex AllowedRegex();
}
