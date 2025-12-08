using UsersService.Domain.Entities.Base;

namespace UsersService.Domain.Entities;

public class EmailChangeToken : BaseEntity, IEntity<int>
{
    public EmailChangeToken(
        Guid userId,
        string newEmail,
        string token)
    {
        UserId = userId;
        NewEmail = newEmail;
        Token = token;
    }
    
    public int Id { get; set; }
    
    public Guid UserId { get; set; }
    public string NewEmail { get; set; }
    public string Token { get; set; }  
    public bool IsUsed { get; set; }
    
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
    
    public virtual User User { get; set; }
}