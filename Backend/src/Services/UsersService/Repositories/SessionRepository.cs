using UsersService.Domain.Contexts;
using UsersService.Domain.Entities;

namespace UsersService.Repositories;

public interface ISessionRepository : IRepository<Session, Guid>
{
    Task<Session?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken,
        Func<IQueryable<Session>, IIncludableQueryable<Session, object>>? include = default,
        bool asTracking = default);
}

public class SessionRepository : Repository<Session, Guid>, ISessionRepository
{
    public SessionRepository(UserServiceDbContext dbContext) : base(dbContext)
    {
    }


    public async Task<Session?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken, 
        Func<IQueryable<Session>, IIncludableQueryable<Session, object>>? include = default,
        bool asTracking = default)
    {
        return asTracking
            ? include is null
                ? await _dbSet.SingleOrDefaultAsync(x => x.Token == token, cancellationToken)
                : await include(_dbSet).AsTracking().SingleOrDefaultAsync(x => x.Token == token, cancellationToken)
            : include is null
                ? await _dbSet.AsNoTracking().SingleOrDefaultAsync(x => x.Token == token, cancellationToken)
                : await include(_dbSet).AsNoTracking().SingleOrDefaultAsync(x => x.Token == token, cancellationToken);

    }
}