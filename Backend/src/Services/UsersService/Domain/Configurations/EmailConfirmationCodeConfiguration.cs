using UsersService.Domain.Entities;

namespace UsersService.Domain.Configurations;

public class EmailConfirmationCodeConfiguration : IEntityTypeConfiguration<EmailConfirmationToken>
{
    public void Configure(EntityTypeBuilder<EmailConfirmationToken> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasQueryFilter(
            p => !p.IsDeleted);
        
        builder.Property(x => x.Token)
            .HasMaxLength(128);
        
        builder.HasOne(e => e.User)
            .WithMany(e => e.EmailConfirmationTokens)
            .HasForeignKey(e => e.UserId);
    }
}