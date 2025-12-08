using UsersService.Repositories;

namespace UsersService.Features.Accounts;

public static class ConfirmEmail
{
    public record Request(string Token);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("accounts/confirm-email", Handler)
                .WithTags("Accounts")
                .WithDescription("Confirm user email")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status409Conflict);
        }

        public static async Task<IResult> Handler(
            Request request,
            IEmailConfirmationTokenRepository emailConfirmationTokenRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            CancellationToken cancellationToken)
        {
            var emailConfirmationToken = await emailConfirmationTokenRepository.GetByTokenAsync(
                token: request.Token,
                cancellationToken: cancellationToken);
            if (emailConfirmationToken == null) return Results.NotFound("Токен подтверджения пользователя не найден!");
            
            if (emailConfirmationToken.IsUsed || emailConfirmationToken.ExpiresAt < DateTime.UtcNow)
                return Results.Conflict("Данный токен больше не доступен!");
            
            var user = await userRepository.GetByIdAsync(
                id: emailConfirmationToken.UserId,
                cancellationToken: cancellationToken);
            if (user == null) return Results.NotFound("Пользователь не найден!");
            
            emailConfirmationToken.IsUsed = true;
            user.IsEmailConfirmed = true;
            
            await emailConfirmationTokenRepository.UpdateAsync(emailConfirmationToken, cancellationToken);
            await userRepository.UpdateAsync(user, cancellationToken);
            
            return Results.NoContent();
        }
    }
}