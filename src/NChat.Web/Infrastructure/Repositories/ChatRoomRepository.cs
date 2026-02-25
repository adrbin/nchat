using Microsoft.EntityFrameworkCore;
using NChat.Web.Application.Abstractions;
using NChat.Web.Domain.Entities;
using NChat.Web.Infrastructure.Persistence;

namespace NChat.Web.Infrastructure.Repositories;

public sealed class ChatRoomRepository : IChatRoomRepository
{
    private readonly NChatDbContext _dbContext;

    public ChatRoomRepository(NChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ChatRoom?> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return _dbContext.ChatRooms.SingleOrDefaultAsync(x => x.Id == roomId, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string roomName, CancellationToken cancellationToken = default)
    {
        return _dbContext.ChatRooms.AnyAsync(x => x.NameValue.ToLower() == roomName.ToLower(), cancellationToken);
    }

    public Task AddAsync(ChatRoom room, CancellationToken cancellationToken = default)
    {
        return _dbContext.ChatRooms.AddAsync(room, cancellationToken).AsTask();
    }

    public Task<List<ChatRoom>> ListAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.ChatRooms.ToListAsync(cancellationToken);
    }
}
