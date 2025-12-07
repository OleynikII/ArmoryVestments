namespace UserService.Api.Repositories;

public interface IEmailChangeTokenRepository : IRepository<EmailChangeToken, int>
{
    Task<EmailChangeToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken,
        Func<IQueryable<EmailChangeToken>, IIncludableQueryable<EmailChangeToken, object>>? include = default,
        bool asTracking = default);
}

public class EmailChangeTokenRepository : Repository<EmailChangeToken, int>, IEmailChangeTokenRepository
{
    public EmailChangeTokenRepository(UserServiceDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<EmailChangeToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken,
        Func<IQueryable<EmailChangeToken>, IIncludableQueryable<EmailChangeToken, object>>? include = default,
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