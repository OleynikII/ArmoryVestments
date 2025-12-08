using UsersService.Repositories;

namespace UsersService.Features.RolePermissions;

public static class GetAll
{
    public record Response(int Id, string Title, string? Description);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("permissions", Handler)
                .WithTags("Permissions")
                .RequireAuthorization(Permissions.RolePermissions.View);
        }
    }

    public static async Task<IResult> Handler(
        string? searchTerm,
        IPermissionRepository permissionRepository,
        CancellationToken cancellationToken = default)
    {
        var permissions = await permissionRepository.GetAllAsync(
            orderBy: permissions => permissions.OrderByDescending(x => x.CreatedAt),
            predicate: !string.IsNullOrWhiteSpace(searchTerm) ? permissions => permissions.Title.Contains(searchTerm) : null,
            cancellationToken: cancellationToken);
        
        var permissionResponse = permissions.Select(permission => 
            new Response(
                Id: permission.Id,
                Title: permission.Title,
                Description: permission.Description)).ToList();
                
        return Results.Ok(permissionResponse);
    }
}