using Microsoft.EntityFrameworkCore;
using NChat.Web.Application.Abstractions;
using NChat.Web.Domain.Entities;
using NChat.Web.Infrastructure.Persistence;

namespace NChat.Web.Infrastructure.Repositories;

public sealed class ChatMessageRepository : IChatMessageRepository
{
    private readonly NChatDbContext _dbContext;

    public ChatMessageRepository(NChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        return _dbContext.ChatMessages.AddAsync(message, cancellationToken).AsTask();
    }

    public Task<List<ChatMessage>> ListRecentByRoomAsync(Guid roomId, int take, CancellationToken cancellationToken = default)
    {
        return _dbContext.ChatMessages
            .Where(x => x.RoomId == roomId)
            .OrderByDescending(x => x.SentAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
