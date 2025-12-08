using UsersService.Domain.Entities;

namespace UsersService.Domain.Configurations;

public class UserRolesConfiguration : IEntityTypeConfiguration<UserRoles>
{
    public void Configure(EntityTypeBuilder<UserRoles> builder)
    {
        builder.HasQueryFilter(
            u => !u.IsDeleted);
        
        builder.SeedData();
    }
}