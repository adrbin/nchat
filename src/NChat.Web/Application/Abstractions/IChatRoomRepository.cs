using NChat.Web.Domain.Entities;

namespace NChat.Web.Application.Abstractions;

public interface IChatRoomRepository
{
    Task<ChatRoom?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string roomName, CancellationToken cancellationToken = default);

    Task AddAsync(ChatRoom room, CancellationToken cancellationToken = default);

    Task<List<ChatRoom>> ListAsync(CancellationToken cancellationToken = default);
}
