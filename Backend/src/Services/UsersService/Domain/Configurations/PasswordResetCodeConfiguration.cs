using UsersService.Domain.Entities;

namespace UsersService.Domain.Configurations;

public class PasswordResetCodeConfiguration : IEntityTypeConfiguration<ResetPasswordCode>
{
    public void Configure(EntityTypeBuilder<ResetPasswordCode> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasQueryFilter(
            p => !p.IsDeleted);
                
        builder.Property(x => x.Code)
            .HasMaxLength(8);
        
        builder.HasOne(e => e.User)
            .WithMany(e => e.PasswordResetCodes)
            .HasForeignKey(e => e.UserId);
    }
}