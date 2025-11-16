namespace UserService.Api.Endpoints.Accounts;

public static class ConfirmEmail
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("accounts/confirm-email", Handler)
                .WithTags("Accounts");
        }

        public static async Task<IResult> Handler(
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            
            return Results.NoContent();
        }
    }
}