using UsersService.Domain.Entities;

namespace UsersService.Domain.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasQueryFilter(
            u => !u.IsDeleted);
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.LastName).HasMaxLength(32);
        builder.Property(u => u.FirstName).HasMaxLength(32);
        builder.Property(u => u.MiddleName).HasMaxLength(32);
        
        builder
            .Property(u => u.Email)
            .HasMaxLength(128);
        builder
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
        
        builder
            .Property(u => u.UserName)
            .HasMaxLength(32);
        builder
            .HasIndex(u => u.UserName)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
        
        builder
            .Property(u => u.PasswordHash)
            .HasMaxLength(72);

        builder
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<UserRoles>(
                l => l
                    .HasOne(ur => ur.Role)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.RoleId),
                r => r
                    .HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId));

        builder.SeedData();
    }
}