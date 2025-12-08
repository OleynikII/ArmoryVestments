using UsersService.Domain.Entities;

namespace UsersService.Domain.DataSeeders;

public static class RolesDataSeeder
{
    internal static void SeedData(this EntityTypeBuilder<Role> builder)
    {
        builder
            .HasData(
                new Role(
                    id: 1,
                    title: Roles.Guest,
                    description: null),
                new Role(
                    id: 2,
                    title: Roles.Moderator,
                    description: null),
                new Role(
                    id: 3,
                    title: Roles.SuperUser,
                    description: null));
    }
}