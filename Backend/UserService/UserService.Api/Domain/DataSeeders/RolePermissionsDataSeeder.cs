namespace UserService.Api.Domain.DataSeeders;

public static class RolePermissionsDataSeeder
{
    public static void SeedData(
        this EntityTypeBuilder<RolePermissions> builder)
    {
        builder
            .HasData(
                new RolePermissions(
                    3, 1),
                new RolePermissions(
                    3, 2),
                new RolePermissions(
                    3, 3),
                new RolePermissions(
                    3, 4),
                new RolePermissions(
                    3, 5),
                new RolePermissions(
                    3, 6),
                
                new RolePermissions(
                    3, 7),
                new RolePermissions(
                    3, 8),
                new RolePermissions(
                    3, 9),
                new RolePermissions(
                    3, 10),
                
                new RolePermissions(
                    3, 11),
                new RolePermissions(
                    3, 12),
                new RolePermissions(
                    3, 13));
    }
}