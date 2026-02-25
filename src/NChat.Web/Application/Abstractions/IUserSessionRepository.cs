using NChat.Web.Domain.Entities;

namespace NChat.Web.Application.Abstractions;

public interface IUserSessionRepository
{
    Task<UserSession?> GetByConnectionIdAsync(string connectionId, CancellationToken cancellationToken = default);

    Task<UserSession?> GetActiveByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<List<UserSession>> ListActiveAsync(CancellationToken cancellationToken = default);

    Task UpsertAsync(UserSession session, CancellationToken cancellationToken = default);
}
