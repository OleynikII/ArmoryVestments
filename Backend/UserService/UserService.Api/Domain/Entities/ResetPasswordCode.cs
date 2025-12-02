namespace UserService.Api.Domain.Entities;

public class ResetPasswordCode : BaseEntity, IEntity<int>
{
    public ResetPasswordCode(
        Guid userId,
        string code)
    {
        UserId = userId;
        Code = code;
    }
    
    public int Id { get; set; }

    public Guid UserId { get; set; }
    public string Code { get; set; }
    public bool IsUsed { get; set; }
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(10);

    public virtual User User { get; set; }
}