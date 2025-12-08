using UsersService.Domain.Contexts;
using UsersService.Domain.Entities;

namespace UsersService.Repositories;

public interface IEmailConfirmationTokenRepository : IRepository<EmailConfirmationToken, int>
{
    Task<EmailConfirmationToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken,
        Func<IQueryable<EmailConfirmationToken>, IIncludableQueryable<EmailConfirmationToken, object>>? include = default,
        bool asTracking = default);
}

public class EmailConfirmationTokenRepository : Repository<EmailConfirmationToken, int>, IEmailConfirmationTokenRepository
{
    public EmailConfirmationTokenRepository(
        UserServiceDbContext dbContext) 
        : base(dbContext)
    {
    }

    public async Task<EmailConfirmationToken?> GetByTokenAsync(
        string token, 
        CancellationToken cancellationToken,
        Func<IQueryable<EmailConfirmationToken>, IIncludableQueryable<EmailConfirmationToken, object>>? include = default, 
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