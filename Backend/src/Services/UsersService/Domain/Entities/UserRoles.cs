using UsersService.Domain.Entities.Base;

namespace UsersService.Domain.Entities;

public class UserRoles : BaseEntity
{
    public UserRoles() { }

    public UserRoles(
        Guid userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
    
    public Guid UserId { get; set; }
    public int RoleId { get; set; }

    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
}