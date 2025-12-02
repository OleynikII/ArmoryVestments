namespace UserService.Api.Endpoints.Accounts;

public static class SendResetPasswordCode
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
            app.MapPost("accounts/send-reset-password-code", Handler)
                .WithTags("Accounts")
                .WithDescription("Send reset password code")
                .Produces(StatusCodes.Status202Accepted)
                .Produces<IList<string>>(StatusCodes.Status400BadRequest);
        }

        public static async Task<IResult> Handler(
            Request request,
            IValidator<Request> validator,
            IResetPasswordCodeHelper resetPasswordCodeHelper,
            IResetPasswordCodeRepository resetPasswordCodeRepository,
            IUserRepository userRepository,
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

            if (!user.IsEmailConfirmed) return Results.Accepted();
            
            var resetPasswordCode = new ResetPasswordCode(
                user.Id,
                code: resetPasswordCodeHelper.GenerateCode());
            await resetPasswordCodeRepository.AddAsync(resetPasswordCode, cancellationToken);

            var emailResetPasswordEvent = new EmailResetPasswordEvent(
                Email: request.Email,
                Code: resetPasswordCode.Code);
            await eventBusPublisher.PublishEmailResetPasswordAsync(emailResetPasswordEvent, cancellationToken);
            
            return Results.Ok(resetPasswordCode.Code);
        }
    }
}