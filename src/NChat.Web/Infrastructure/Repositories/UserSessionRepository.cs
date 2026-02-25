using Microsoft.EntityFrameworkCore;
using NChat.Web.Application.Abstractions;
using NChat.Web.Domain.Entities;
using NChat.Web.Infrastructure.Persistence;

namespace NChat.Web.Infrastructure.Repositories;

public sealed class UserSessionRepository : IUserSessionRepository
{
    private readonly NChatDbContext _dbContext;

    public UserSessionRepository(NChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UserSession?> GetByConnectionIdAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        return _dbContext.UserSessions.SingleOrDefaultAsync(x => x.ConnectionId == connectionId, cancellationToken);
    }

    public Task<UserSession?> GetActiveByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return _dbContext.UserSessions.SingleOrDefaultAsync(
            x => x.IsActive && x.UsernameValue != null && x.UsernameValue.ToLower() == username.ToLower(),
            cancellationToken);
    }

    public Task<List<UserSession>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.UserSessions.Where(x => x.IsActive).ToListAsync(cancellationToken);
    }

    public async Task UpsertAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.UserSessions.AnyAsync(x => x.Id == session.Id, cancellationToken);
        if (!exists)
        {
            await _dbContext.UserSessions.AddAsync(session, cancellationToken);
        }
    }
}
