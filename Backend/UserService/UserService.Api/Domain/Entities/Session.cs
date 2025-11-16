namespace UserService.Api.Domain.Entities;

public class Session : BaseEntity, IEntity<Guid>
{
    public Session() {  }

    public Session(
        Guid userId,
        string token,
        DateTime expiresAt)
    {
        UserId = userId;    
        Token = token;
        ExpiresAt = expiresAt;
    }
    
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    

    public virtual User User { get; set; }
}