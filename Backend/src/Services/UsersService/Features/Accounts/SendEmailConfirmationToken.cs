using UsersService.Domain.Entities;
using UsersService.Helpers;
using UsersService.Repositories;
using UsersService.Services;

namespace UsersService.Features.Accounts;

public static class SendEmailConfirmationToken
{
    public record Request(string Email);
    
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Почта не может быть пустой!")
                .EmailAddress().WithMessage("Некорректный формат почты!")
                .MaximumLength(128).WithMessage("Длина почты не должна превышать 32 символов!");
        }
    }
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("accounts/send-email-confirmation-token", Handler)
                .WithTags("Accounts")
                .WithDescription("Send confirmation token")
                .Produces(StatusCodes.Status202Accepted)
                .Produces<IList<string>>(StatusCodes.Status400BadRequest);
        }

        public static async Task<IResult> Handler(
            Request request,
            IValidator<Request> validator,
            IUserRepository userRepository,
            IEmailConfirmationTokenRepository emailConfirmationTokenRepository,
            IHttpContextAccessor httpContextAccessor,
            IEmailTokenHelper emailTokenHelper,
            IEventBusPublisher eventBusPublisher,
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            
            var user = await userRepository.GetByEmailAsync(
                email: request.Email,
                cancellationToken: cancellationToken);
            if (user == null) return Results.Accepted();
            
            if (user.IsEmailConfirmed) return Results.Accepted();
            
            var emailConfirmationToken = new EmailConfirmationToken(
                userId: user.Id,
                token: emailTokenHelper.GenerateTokenByEmail(user.Email));
            
            await emailConfirmationTokenRepository.AddAsync(emailConfirmationToken, cancellationToken);

            var emailConfirmationEvent = new EmailConfirmationEvent(
                Email: user.Email,
                Token: emailConfirmationToken.Token);

            await eventBusPublisher.PublishEmailConfirmationAsync(emailConfirmationEvent, cancellationToken);
            
            return Results.Accepted();
        }
    }
}