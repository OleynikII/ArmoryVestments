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
                    3, 6));
    }
}