namespace UserService.Api.Endpoints.Accounts;

public static class ResetPassword
{
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("accounts/reset-password", Handler)
                .WithTags("Accounts")
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
        if (user == null) return Results.NotFound();
        
        
        return Results.NoContent();
    }
}