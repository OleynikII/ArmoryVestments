using UsersService.Domain.Entities;
using UsersService.Helpers;
using UsersService.Repositories;
using UsersService.Services;

namespace UsersService.Features.Accounts;

public static class SendEmailChangeToken
{
    public record Request(
        string NewEmail);

    public sealed class Validator : AbstractValidator<Request>
    {
        private const string NameRegex = @"^[\p{L}\-\s']+$";
        private const string UsernameRegex = @"^[a-zA-Z0-9_]+$";

        public Validator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("Почта не может быть пустой!")
                .EmailAddress().WithMessage("Некорректный формат почты!")
                .MaximumLength(128).WithMessage("Длина почты не должна превышать 32 символов!");
        }

        private bool BeValidUsername(string username)
        {
            if (string.IsNullOrEmpty(username)) return true;
            return !username.All(char.IsDigit);
        }
        

        private bool BeValidNamePart(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return true;
            return name.Trim().Length > 0 && name.Replace("-", "").Replace("'", "").Trim().Length > 0;
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("accounts/send-email-change-token", Handler)
                .WithTags("Accounts")
                .WithDescription("Send email change token")
                .Produces(StatusCodes.Status202Accepted)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<IList<string>>(StatusCodes.Status400BadRequest)
                .Produces<string>(StatusCodes.Status409Conflict)
                .RequireAuthorization();
        }
    }
    
    public static async Task<IResult> Handler(
        Request request,
        IHttpContextAccessor httpContextAccessor,
        IEmailChangeTokenRepository emailChangeTokenRepository,
        IEmailTokenHelper emailTokenHelper,
        IUserRepository userRepository,
        IValidator<Request> validator,
        IEventBusPublisher eventBusPublisher,
        CancellationToken cancellationToken = default)
    {
        var currentUserIdStr = httpContextAccessor.HttpContext!.User.FindFirstValue(ApplicationClaimTypes.UserId);
        if (string.IsNullOrWhiteSpace(currentUserIdStr)) return Results.Unauthorized();
        
        var userId = Guid.Parse(currentUserIdStr);
        var user = await userRepository.GetByIdAsync(
            id: userId,
            cancellationToken: cancellationToken);
        if (user == null) return Results.NotFound("Пользователь не найден!");
        
        var validatorResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validatorResult.IsValid) 
            return Results.BadRequest(validatorResult.Errors.Select(x => x.ErrorMessage).ToList());

        if (user.Email == request.NewEmail)
            return Results.Conflict("Новая почта должна отличатся от старой!");
        
        var isExistByEmail = await userRepository.IsExistByEmailForUpdateAsync(
            userId: user.Id,
            email: request.NewEmail,
            cancellationToken: cancellationToken);
        if (isExistByEmail)
            return Results.Conflict("Пользователь с данной почтой уже существует!");

        var emailChangeToken = new EmailChangeToken(
            userId: user.Id,
            newEmail: request.NewEmail,
            token: emailTokenHelper.GenerateTokenByEmail(request.NewEmail));
        await emailChangeTokenRepository.AddAsync(emailChangeToken, cancellationToken);

        var emailChangeEvent = new EmailChangeEvent(
            NewEmail: request.NewEmail,
            Token: emailChangeToken.Token);
        await eventBusPublisher.PublishEmailChangeAsync(emailChangeEvent, cancellationToken);
        
        return Results.Accepted();
    }
}