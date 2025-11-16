namespace UserService.Api.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByUserNameAsync(
        string userName, 
        CancellationToken cancellationToken,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = default,
        bool asTracking = default);

    Task<bool> IsExistByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<bool> IsExistByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> IsExistByUserNameForUpdateAsync(Guid userId, string userName, CancellationToken cancellationToken);
    Task<bool> IsExistByEmailForUpdateAsync(Guid userId, string email, CancellationToken cancellationToken);
}

public class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(UserServiceDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> GetByUserNameAsync(
        string userName, 
        CancellationToken cancellationToken,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = default,
        bool asTracking = default)
    {
        return asTracking
            ? include is null
                ? await _dbSet.SingleOrDefaultAsync(x => Equals(x.UserName, userName), cancellationToken)
                : await include(_dbSet).AsTracking().SingleOrDefaultAsync(x => Equals(x.UserName, userName), cancellationToken)
            : include is null
                ? await _dbSet.AsNoTracking().SingleOrDefaultAsync(x => Equals(x.UserName, userName), cancellationToken)
                : await include(_dbSet).AsNoTracking().SingleOrDefaultAsync(x => Equals(x.UserName, userName), cancellationToken);

    }

    public async Task<bool> IsExistByUserNameAsync(string userName, CancellationToken cancellationToken) =>
        await _dbSet.AnyAsync(x => Equals(x.UserName, userName), cancellationToken);
    
    public async Task<bool> IsExistByEmailAsync(string email, CancellationToken cancellationToken) =>
         await _dbSet.AnyAsync(x => Equals(x.Email, email), cancellationToken);
    
    public async Task<bool> IsExistByUserNameForUpdateAsync(Guid userId, string userName, CancellationToken cancellationToken) =>
        await _dbSet.AnyAsync(x => Equals(x.UserName, userName) && !Equals(x.Id, userId), cancellationToken);

    public async Task<bool> IsExistByEmailForUpdateAsync(Guid userId, string email, CancellationToken cancellationToken)=>
        await _dbSet.AnyAsync(x => Equals(x.Email, email) && !Equals(x.Id, userId), cancellationToken);
}