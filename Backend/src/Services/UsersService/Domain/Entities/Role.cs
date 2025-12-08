using UsersService.Domain.Entities.Base;

namespace UsersService.Domain.Entities;

public class Role : BaseEntity, IEntity<int>
{
    public Role() {  }

    public Role(int id, string title, string? description)
    {
        Id = id;
        Title = title;
        Description = description;
    }
    
    public Role(string title, string? description)
    {
        Title = title;
        Description = description;
    }
    
    public int Id { get; set; }

    public string Title { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; }
    public virtual ICollection<RolePermissions> RolePermissions { get; set; }
    
    public virtual ICollection<User> Users { get; set; } = [];
    public virtual ICollection<UserRoles> UserRoles { get; set; } = [];
}