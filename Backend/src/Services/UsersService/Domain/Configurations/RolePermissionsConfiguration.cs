using UsersService.Domain.Entities;

namespace UsersService.Domain.Configurations;

public class RolePermissionsConfiguration 
    : IEntityTypeConfiguration<RolePermissions>
{
    public void Configure(EntityTypeBuilder<RolePermissions> builder)
    {
        builder.HasQueryFilter(
            u => !u.IsDeleted);
        
        builder.SeedData();
    }
}