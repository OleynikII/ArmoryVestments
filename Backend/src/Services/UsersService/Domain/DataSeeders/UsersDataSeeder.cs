using UsersService.Domain.Entities;

namespace UsersService.Domain.DataSeeders;

public static class UsersDataSeeder
{
    public static void SeedData(this EntityTypeBuilder<User> builder)
    {
        builder.HasData(
            new User(
                id: Guid.Parse("f4ba8595-1cdd-4b7e-8208-389ce8f71342"),
                lastName: "Олейник",
                firstName: "Илья",
                middleName: "Игоревич",
                email: "ilya123@gmail.com",
                userName: "ilya123",
                passwordHash: BCrypt.Net.BCrypt.EnhancedHashPassword("Gg12032003"),
                isEmailConfirmed: false,
                activateUser: true),
            new User(
                id: Guid.Parse("79054f39-159e-440b-8703-752ef50a2f30"),
                lastName: "Файзрахманов",
                firstName: "Салават",
                middleName: "Ранилевич",
                email: "sala123@gmail.com",
                userName: "sala123",
                passwordHash: BCrypt.Net.BCrypt.EnhancedHashPassword("Gg12032003"),
                isEmailConfirmed: false,
                activateUser: true),
            new User(
                id: Guid.Parse("a09a029b-28e3-4419-aaee-8bf59276a40e"),
                lastName: "ТестоваяФамилия",
                firstName: "ТестовоеИмя",
                middleName: null,
                email: "guest123@gmail.com",
                userName: "guest123",
                passwordHash: BCrypt.Net.BCrypt.EnhancedHashPassword("Gg12032003"),
                isEmailConfirmed: false,
                activateUser: true));
    }
}