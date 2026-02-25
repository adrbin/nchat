namespace NChat.Web.Presentation.Contracts;

public sealed record ClaimNameRequest(string Username);

public sealed record CreateRoomRequest(string Name);
