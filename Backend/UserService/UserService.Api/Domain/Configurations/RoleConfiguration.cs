namespace UserService.Api.Domain.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .HasQueryFilter(
                r => !r.IsDeleted);

        builder.HasKey(r => r.Id);

        builder
            .Property(r => r.Title)
            .HasMaxLength(32);
        builder
            .HasIndex(r => r.Title)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder
            .Property(r => r.Description)
            .HasMaxLength(512);

        builder
            .HasMany(r => r.Permissions)
            .WithMany(r => r.Roles)
            .UsingEntity<RolePermissions>(
                l => l
                    .HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId),
                r => r
                    .HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId));
        
        builder.SeedData();
    }
}