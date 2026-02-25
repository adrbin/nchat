namespace NChat.Web.Application.Dto;

public sealed record ClaimUsernameResult(bool Ok, string? NormalizedUsername, string? Reason);

public sealed record RoomSummaryDto(Guid Id, string Name, int UserCount);

public sealed record PresenceUserDto(string Username, DateTimeOffset ConnectedAt);

public sealed record MessageDto(Guid Id, Guid RoomId, string Username, string Content, DateTimeOffset SentAt);
