using Shared.Claims.Permission;

namespace UserService.Api.Domain.DataSeeders;

public static class PermissionDataSeeder
{
    public static void SeedData(this EntityTypeBuilder<Permission> builder)
    {
        builder
            .HasData(
                new Permission(
                    1,
                    Permissions.Users.Get,
                    Permissions.Users.Get),
                new Permission(
                    2,
                    Permissions.Users.Create,
                    Permissions.Users.Create),
                new Permission(
                    3,
                    Permissions.Users.Update,
                    Permissions.Users.Update),
                new Permission(
                    4,
                    Permissions.Users.Delete,
                    Permissions.Users.Delete),
                new Permission(
                    5,
                    Permissions.Users.Export,
                    Permissions.Users.Export),
                new Permission(
                    6,
                    Permissions.Users.Import,
                    Permissions.Users.Import));
    }
}