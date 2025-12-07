namespace UserService.Api.Domain.DataSeeders;

public static class PermissionDataSeeder
{
    public static void SeedData(this EntityTypeBuilder<Permission> builder)
    {
        builder
            .HasData(
                new Permission(
                    1,
                    Permissions.Users.View,
                    Permissions.Users.View),
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
                    Permissions.Users.Import),
                
                new Permission(
                    7,
                    Permissions.Roles.View,
                    Permissions.Roles.View),
                new Permission(
                    8,
                    Permissions.Roles.Create,
                    Permissions.Roles.Create),
                new Permission(
                    9,
                    Permissions.Roles.Update,
                    Permissions.Roles.Update),
                new Permission(
                    10, 
                    Permissions.Roles.Delete,
                    Permissions.Roles.Delete),
                
                new Permission(
                    11,
                    Permissions.RolePermissions.View,
                    Permissions.RolePermissions.View),
                new Permission(
                    12,
                    Permissions.RolePermissions.Create,
                    Permissions.RolePermissions.Create),
                new Permission(
                    13,
                    Permissions.RolePermissions.Delete,
                    Permissions.RolePermissions.Delete));
    }
}