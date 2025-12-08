using UsersService.Repositories;

namespace UsersService.Features.Accounts;

public static class ResetPassword
{
    public record Request(string Code, string NewPassword, string ConfirmNewPassword);

    public sealed class Validation : AbstractValidator<Request>
    {
        public Validation()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Код смены пароля не может быть пустым!");
            
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Новый пароль обязателен для заполнения!")
                .MinimumLength(8).WithMessage($"Новый пароль должен содержать минимум 8 символов")
                .Matches("[A-Z]").WithMessage("Новый пароль должен содержать хотя бы одну заглавную букву!")
                .Matches("[a-z]").WithMessage("Новый пароль должен содержать хотя бы одну строчную букву!")
                .Matches("[0-9]").WithMessage("Новый пароль должен содержать хотя бы одну цифру!");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Повтор пароля обязателен для заполнения!")
                .Equal(x => x.NewPassword).WithMessage("Пароли не совпадают!");
        }
    }
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("accounts/reset-password", Handler)
                .WithTags("Accounts")
                .WithDescription("Reset password")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<IList<string>>(StatusCodes.Status400BadRequest)
                .Produces<string>(StatusCodes.Status404NotFound);
        }
    }

    public static async Task<IResult> Handler(
        Request request,
        IResetPasswordCodeRepository resetPasswordCodeRepository,
        IUserRepository userRepository,
        IValidator<Request> validator,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        var resetPasswordCode = await resetPasswordCodeRepository.GetByCodeAsync(
            code: request.Code,
            cancellationToken: cancellationToken);
        if (resetPasswordCode == null) return Results.NotFound("Код смены пароля не найден!");
        
        if (resetPasswordCode.IsUsed || resetPasswordCode.ExpiresAt < DateTime.UtcNow)
            return Results.Conflict("Данный код больше не доступен!");
        
        var user = await userRepository.GetByIdAsync(
            resetPasswordCode.UserId,
            cancellationToken: cancellationToken);
        if (user == null) return Results.NotFound("Пользователь не найден!");
        
        resetPasswordCode.IsUsed = true;
        user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.NewPassword);
        
        await resetPasswordCodeRepository.UpdateAsync(resetPasswordCode, cancellationToken);
        await userRepository.UpdateAsync(user, cancellationToken);
        
        return Results.NoContent();
    }
}