namespace UserService.Api.Domain.Configurations;

public class EmailChangeTokenConfiguration : IEntityTypeConfiguration<EmailChangeToken>
{
    public void Configure(EntityTypeBuilder<EmailChangeToken> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasQueryFilter(
            p => !p.IsDeleted);
        
        builder
            .Property(x => x.NewEmail)
            .HasMaxLength(128);
        
        builder
            .Property(x => x.Token)
            .HasMaxLength(128);
        
        builder.HasOne(e => e.User)
            .WithMany(e => e.EmailChangeTokens)
            .HasForeignKey(e => e.UserId);
    }
}