using System.Reflection;

namespace UserService.Api.Domain.Contexts;

public class UserServiceDbContext(
    DbContextOptions<UserServiceDbContext> options) 
    : DbContext(options)
{
    public DbSet<ResetPasswordCode> PasswordResetCodes => Set<ResetPasswordCode>();
    public DbSet<EmailConfirmationToken> EmailConfirmationTokens => Set<EmailConfirmationToken>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}