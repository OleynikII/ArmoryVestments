namespace UserService.Api.Domain.Entities;

public class RolePermissions : BaseEntity
{
    public RolePermissions() { }

    public RolePermissions(
        int roleId,
        int permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
    
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    public virtual Role Role { get; set; }
    public virtual Permission Permission { get; set; }
}