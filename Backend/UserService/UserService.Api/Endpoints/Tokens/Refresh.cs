namespace UserService.Api.Endpoints.Tokens;

public static class Refresh
{
    public record Request(string RefreshToken);
    public record Response(string AccessToken, string RefreshToken);
    
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Рефреш токен не может быть пустым!");
        }
    }
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("tokens/refresh", Handler)
                .WithTags("Tokens");
        }
    }

    private static async Task<IResult> Handler(
        Request request,
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IValidator<Request> validator,
        IJwtHelper jwtHelper,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

        var session = await sessionRepository.GetByTokenAsync(
            token: request.RefreshToken,
            cancellationToken: cancellationToken);
        if (session == null || session.ExpiresAt < DateTime.UtcNow) return Results.Unauthorized();

        var user = await userRepository.GetByIdAsync(
            id: session.UserId,
            include: user => user.Include(u => u.Roles).ThenInclude(r => r.Permissions),
            cancellationToken: cancellationToken);
        if (user == null) return Results.Unauthorized();
        
        var accessToken = jwtHelper.GenerateJwtToken(user);
        var refreshToken = jwtHelper.GenerateRefreshToken();
        
        await sessionRepository.DeleteAsync(session, cancellationToken);
        
        var newSession = new Session(
            user.Id,
            refreshToken,
            DateTime.UtcNow.AddDays(30));
        await sessionRepository.AddAsync(newSession, cancellationToken); 
        
        var response = new Response(accessToken, refreshToken);
        
        return Results.Ok(response);
    }
}