using UsersService.Repositories;

namespace UsersService.Features.Roles;

public class GetAll
{
    public record Response(int Id, string Title, string? Description);
    
    public class Endpoint :IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("roles", Handler)
                .WithTags("Roles")
                .WithDescription("Get all roles")
                .Produces<IList<Response>>()
                .RequireAuthorization(Permissions.Roles.View);
        }
    }

    public static async Task<IResult> Handler(
        string? searchTerm,
        IRoleRepository roleRepository,
        CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetAllAsync(
            orderBy: roles => roles.OrderByDescending(x => x.CreatedAt),
            predicate: !string.IsNullOrWhiteSpace(searchTerm) ? roles => roles.Title.Contains(searchTerm) : null,
            cancellationToken: cancellationToken);
        
        var rolesResponse = roles.Select(role =>
            new Response(
                Id: role.Id, 
                Title: role.Title,
                Description: role.Description)).ToList();
        
        return Results.Ok(rolesResponse);
    }
}