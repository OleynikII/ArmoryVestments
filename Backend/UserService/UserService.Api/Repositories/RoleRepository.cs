namespace UserService.Api.Repositories;

public interface IRoleRepository : IRepository<Role, int>
{
}

public class RoleRepository : Repository<Role, int>, IRoleRepository
{
    public RoleRepository(UserServiceDbContext dbContext) : base(dbContext)
    {
    }
}