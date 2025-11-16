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
                .WithTags("Tokens");
        }
    }

    private static async Task<IResult> Handler(
        Request request,
        ISessionRepository sessionRepository,
        IValidator<Request> validator,
        CancellationToken cancellationToken = default)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid) return Results.BadRequest(result.Errors);

        var session = await sessionRepository.GetByTokenAsync(
            token: request.RefreshToken,
            cancellationToken: cancellationToken);
        if (session == null) return Results.Unauthorized();
        
        await sessionRepository.DeleteAsync(session, cancellationToken);
        
        return Results.NoContent();
    }
}