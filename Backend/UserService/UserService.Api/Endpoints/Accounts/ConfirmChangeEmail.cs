namespace UserService.Api.Endpoints.Accounts;

public class ConfirmChangeEmail
{
    public record Request(string Token);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("accounts/confirm-change-email", Handler)
                .WithTags("Accounts")
                .WithDescription("Confirm user email")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status409Conflict);
        }

        public static async Task<IResult> Handler(
            Request request,
            IEmailChangeTokenRepository emailChangeTokenRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            CancellationToken cancellationToken)
        {
            var emailChangeToken = await emailChangeTokenRepository.GetByTokenAsync(
                token: request.Token,
                cancellationToken: cancellationToken);
            if (emailChangeToken == null) return Results.NotFound("Токен подтверджения смены почты не найден!");
            
            if (emailChangeToken.IsUsed || emailChangeToken.ExpiresAt < DateTime.UtcNow)
                return Results.Conflict("Данный токен больше не доступен!");
            
            var user = await userRepository.GetByIdAsync(
                id: emailChangeToken.UserId,
                cancellationToken: cancellationToken);
            if (user == null) return Results.NotFound("Пользователь не найден!");
            
            emailChangeToken.IsUsed = true;
            
            user.Email = emailChangeToken.NewEmail;
            user.IsEmailConfirmed = true;
            
            await emailChangeTokenRepository.UpdateAsync(emailChangeToken, cancellationToken);
            await userRepository.UpdateAsync(user, cancellationToken);
            
            return Results.NoContent();
        }
    }
}