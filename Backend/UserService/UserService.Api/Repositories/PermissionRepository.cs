namespace UserService.Api.Repositories;

public interface IPermissionRepository : IRepository<Permission, int>
{
}

public class PermissionRepository : Repository<Permission, int>, IPermissionRepository
{
    public PermissionRepository(UserServiceDbContext dbContext) : base(dbContext)
    {
    }
}