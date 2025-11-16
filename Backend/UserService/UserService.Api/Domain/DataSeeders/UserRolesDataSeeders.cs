namespace UserService.Api.Domain.DataSeeders;

public static class UserRolesDataSeeders
{
    public static void SeedData(
        this EntityTypeBuilder<UserRoles> builder)
    {
        builder
            .HasData(
                new UserRoles(
                    userId: Guid.Parse("f4ba8595-1cdd-4b7e-8208-389ce8f71342"),
                    roleId: 3),
                new UserRoles(
                    userId: Guid.Parse("79054f39-159e-440b-8703-752ef50a2f30"),
                    roleId: 2));
                new UserRoles(
                    userId: Guid.Parse("a09a029b-28e3-4419-aaee-8bf59276a40e"), 
                    roleId: 1);
    }
}