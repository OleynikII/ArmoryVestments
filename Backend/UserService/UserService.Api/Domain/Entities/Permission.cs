namespace UserService.Api.Domain.Entities;

public class Permission : BaseEntity, IEntity<int>
{
    public Permission() { }
    
    public Permission(
        int id,
        string title,
        string? description)
    {
        Id = id;
        Title = title;
        Description = description;
    }
    
    public Permission(
        string title,
        string? description)
    {
        Title = title;
        Description = description;  
    }

    
    public int Id { get; set; }
    
    public string Title { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = [];
    public virtual ICollection<RolePermissions> RolePermissions { get; set; } = [];

}