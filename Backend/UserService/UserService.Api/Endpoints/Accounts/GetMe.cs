using System.Security.Claims;

namespace UserService.Api.Endpoints.Accounts;

public static class GetMe
{
    public record Response(
        Guid Id,
        string LastName,
        string FirstName,
        string? MiddleName,
        string UserName,
        string Email,
        bool IsEmailConfirmed,
        bool IsActive);
    
    public class EndPoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("accounts/me", Handler)
                .WithTags("Accounts")
                .WithDescription("Get authenticated user")
                .Produces<Response>()
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces<string>(StatusCodes.Status404NotFound)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        var currentUserIdStr = httpContextAccessor.HttpContext!.User.FindFirstValue(ApplicationClaimTypes.UserId);
        if (string.IsNullOrWhiteSpace(currentUserIdStr)) return Results.Unauthorized();
        
        var userId = Guid.Parse(currentUserIdStr);
        var user = await userRepository.GetByIdAsync(
            id: userId,
            cancellationToken: cancellationToken);
        if (user == null) return Results.NotFound("Пользователь не найден!");
        
        var response = new Response(
            Id: user.Id,
            LastName: user.LastName,
            FirstName: user.FirstName,
            MiddleName: user.MiddleName,
            UserName: user.UserName, 
            Email: user.Email,
            IsEmailConfirmed: user.IsEmailConfirmed,
            IsActive: user.IsActive);
        
        return Results.Ok(response);
    }
}