using UsersService.Repositories;

namespace UsersService.Features.Accounts;

public static class UpdatePassword
{
    public record Request(
        string OldPassword,
        string NewPassword,
        string ConfirmNewPassword);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Текущий пароль обязателен для заполнения!")
                .MinimumLength(8).WithMessage($"Длина текущего пароля должна быть не менее 8 символов!");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Новый пароль обязателен для заполнения!")
                .MinimumLength(8).WithMessage($"Новый пароль должен содержать минимум 8 символов")
                .Matches("[A-Z]").WithMessage("Новый пароль должен содержать хотя бы одну заглавную букву!")
                .Matches("[a-z]").WithMessage("Новый пароль должен содержать хотя бы одну строчную букву!")
                .Matches("[0-9]").WithMessage("Новый пароль должен содержать хотя бы одну цифру!")
                .NotEqual(x => x.OldPassword).WithMessage("Новый пароль должен отличаться от текущего!");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Повтор пароля обязателен для заполнения!")
                .Equal(x => x.NewPassword).WithMessage("Пароли не совпадают!");
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("accounts/update-password", Handler)
                .WithTags("Accounts")
                .WithDescription("Update authenticated user password")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces<string>(StatusCodes.Status404NotFound)
                .Produces<IList<string>>(StatusCodes.Status400BadRequest)
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(
        Request request,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        IValidator<Request> validator,
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
        
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        
        await userRepository.UpdateAsync(user, cancellationToken);
        
        return Results.NoContent();
    }
}