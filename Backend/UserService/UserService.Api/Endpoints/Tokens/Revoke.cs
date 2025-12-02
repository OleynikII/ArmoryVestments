namespace UserService.Api.Endpoints.Tokens;

public static class Revoke
{
    public record Request(string RefreshToken);

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
            app.MapPost("tokens/revoke", Handler)
                .WithTags("Tokens")
                .WithDescription("Logout user")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<IList<string>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handler(
        Request request,
        ISessionRepository sessionRepository,
        IValidator<Request> validator,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid) 
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        var session = await sessionRepository.GetByTokenAsync(
            token: request.RefreshToken,
            cancellationToken: cancellationToken);
        if (session == null) return Results.Unauthorized();
        
        await sessionRepository.DeleteAsync(session, cancellationToken);
        
        return Results.NoContent();
    }
}