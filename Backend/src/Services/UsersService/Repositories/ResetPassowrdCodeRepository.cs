using UsersService.Domain.Contexts;
using UsersService.Domain.Entities;

namespace UsersService.Repositories;

public interface IResetPasswordCodeRepository : IRepository<ResetPasswordCode, int>
{
    Task<ResetPasswordCode?> GetByCodeAsync(
        string code, 
        CancellationToken cancellationToken,
        Func<IQueryable<ResetPasswordCode>, IIncludableQueryable<ResetPasswordCode, object>>? include = default,
        bool asTracking = default);
}

public class ResetPasswordCodeRepository : Repository<ResetPasswordCode, int>, IResetPasswordCodeRepository
{
    public ResetPasswordCodeRepository(UserServiceDbContext dbContext) : base(dbContext)
    {
    }


    public async Task<ResetPasswordCode?> GetByCodeAsync(
        string code, 
        CancellationToken cancellationToken, 
        Func<IQueryable<ResetPasswordCode>, IIncludableQueryable<ResetPasswordCode, object>>? include = default,
        bool asTracking = default)
    {
        return asTracking
            ? include is null
                ? await _dbSet.SingleOrDefaultAsync(x => x.Code == code, cancellationToken)
                : await include(_dbSet).AsTracking().SingleOrDefaultAsync(x => x.Code == code, cancellationToken)
            : include is null
                ? await _dbSet.AsNoTracking().SingleOrDefaultAsync(x => x.Code == code, cancellationToken)
                : await include(_dbSet).AsNoTracking().SingleOrDefaultAsync(x => x.Code == code, cancellationToken);

    }
}