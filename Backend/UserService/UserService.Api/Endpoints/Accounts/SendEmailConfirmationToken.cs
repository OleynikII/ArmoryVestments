namespace UserService.Api.Endpoints.Accounts;

public static class SendEmailConfirmationToken
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("accounts/send-email-confirmation-token", Handler)
                .WithTags("Accounts")
                .WithDescription("Send confirmation token")
                .RequireAuthorization();
        }

        public static async Task<IResult> Handler(
            IUserRepository userRepository,
            IEmailConfirmationTokenRepository emailConfirmationTokenRepository,
            IHttpContextAccessor httpContextAccessor,
            IEmailConfirmationTokenHelper emailConfirmationTokenHelper,
            IEventBusPublisher eventBusPublisher,
            CancellationToken cancellationToken)
        {
            var currentUserIdStr = httpContextAccessor.HttpContext!.User.FindFirstValue(ApplicationClaimTypes.UserId);
            if (string.IsNullOrWhiteSpace(currentUserIdStr)) return Results.Unauthorized();
        
            var userId = Guid.Parse(currentUserIdStr);
            var user = await userRepository.GetByIdAsync(
                id: userId,
                cancellationToken: cancellationToken);
            if (user == null) return Results.NotFound("Пользователь не найден!");
            
            if (user.IsEmailConfirmed) return Results.Conflict("Почта уже потдверждена!");
            
            var emailConfirmationToken = new EmailConfirmationToken(
                userId: userId,
                token: emailConfirmationTokenHelper.GenerateTokenByEmail(user.Email));
            
            await emailConfirmationTokenRepository.AddAsync(emailConfirmationToken, cancellationToken);

            var emailConfirmationEvent = new EmailConfirmationEvent(
                Email: user.Email,
                Token: emailConfirmationToken.Token);

            await eventBusPublisher.PublishEmailConfirmationAsync(emailConfirmationEvent, cancellationToken);
            
            return Results.Accepted();
        }
    }
}