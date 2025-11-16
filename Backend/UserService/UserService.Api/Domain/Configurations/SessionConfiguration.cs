namespace UserService.Api.Domain.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder
            .HasQueryFilter(
                s => !s.IsDeleted);

        builder.HasKey(s => s.Id);

        builder
            .Property(s => s.Token);
        builder
            .Property(s => s.ExpiresAt);

        builder
            .HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId);
    }
}