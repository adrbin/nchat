using NChat.Web.Domain.Entities;

namespace NChat.Web.Application.Abstractions;

public interface IChatMessageRepository
{
    Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default);

    Task<List<ChatMessage>> ListRecentByRoomAsync(Guid roomId, int take, CancellationToken cancellationToken = default);
}
