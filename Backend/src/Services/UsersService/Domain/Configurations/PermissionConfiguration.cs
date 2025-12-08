using UsersService.Domain.Entities;

namespace UsersService.Domain.Configurations;

public class PermissionConfiguration
    : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);

        builder
            .Property(p => p.Title)
            .HasMaxLength(64);
        builder
            .HasIndex(p => p.Title)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
        builder.HasQueryFilter(
            p => !p.IsDeleted);

        builder
            .Property(p => p.Description)
            .HasMaxLength(512);
        
        builder
            .HasMany(p => p.Roles)
            .WithMany(r => r.Permissions)
            .UsingEntity<RolePermissions>(
                l => l
                    .HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId),
                r => r
                    .HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId));
        
        builder.SeedData();
    }
}