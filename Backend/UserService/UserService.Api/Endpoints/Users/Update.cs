namespace UserService.Api.Endpoints.Users;

public static class Update
{
    public record Request(
        string LastName, 
        string FirstName,
        string MiddleName,
        string UserName,
        string Email,
        string NewPassword,
        bool IsChangingPassword);
    
    public sealed class Validator : AbstractValidator<Request>
    {
        private const string NameRegex = @"^[\p{L}\-\s']+$";
        private const string UsernameRegex = @"^[a-zA-Z0-9_]+$";
        
        public Validator()
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна для заполнения!")
                .MaximumLength(32).WithMessage("Длина фамилии не должна превышать 32 символов!")
                .Matches(NameRegex).WithMessage("Фамилия содержит недопустимые символы!")
                .Must(BeValidNamePart).WithMessage("Фамилия не может состоять только из пробелов или дефисов!");
            
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя обязательно для заполнения!")
                .MaximumLength(32).WithMessage($"Длина имени не должна превышать 32 символов!")
                .Matches(NameRegex).WithMessage("Имя содержит недопустимые символы!")
                .Must(BeValidNamePart).WithMessage("Имя не может состоять только из пробелов или дефисов!");

            RuleFor(x => x.MiddleName)
                .MaximumLength(32).WithMessage($"Длина отчества не должна превышать 32 символов!")
                .Matches(NameRegex).When(x => !string.IsNullOrEmpty(x.MiddleName))
                .WithMessage("Отчество содержит недопустимые символы!")
                .Must(BeValidNamePart).When(x => !string.IsNullOrEmpty(x.MiddleName))
                .WithMessage("Отчество не может состоять только из пробелов или дефисов!");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Имя пользователя обязательно для заполнения!")
                .MaximumLength(32).WithMessage("Длина логина не должна превышать 32 символов!")
                .Matches(UsernameRegex).WithMessage("Логин может содержать только латинские буквы, цифры и подчёркивания!")
                .Must(BeValidUsername).WithMessage("Логин не может состоять только из цифр!");
            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Почта не может быть пустой!")
                .EmailAddress().WithMessage("Некорректный формат почты!")
                .MaximumLength(128).WithMessage("Длина почты не должна превышать 32 символов!!");
        
            When(x => x.IsChangingPassword, () =>
            {
                RuleFor(x => x.NewPassword)
                    .NotEmpty().WithMessage("Пароль обязателен при изменении пароля!")
                    .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов!")
                    .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву!")
                    .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву!")
                    .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру!");
            });
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
            app.MapPut("users/{userId:guid}", Handler)
                .WithTags("Users")
                .WithDescription("Update user by id")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<IList<string>>(StatusCodes.Status400BadRequest)
                .Produces<string>(StatusCodes.Status404NotFound)
                .RequireAuthorization(Permissions.Users.Update);
        }
    }
    
    public static async Task<IResult> Handler(
        Guid userId,
        Request request,
        IValidator<Request> validator,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty) 
            return Results.BadRequest( new List<string>{ "Некорректный формат идентификатора!" });

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid) 
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        
        var isExistByUserNameForUpdate = await userRepository.IsExistByUserNameForUpdateAsync(
            userId: userId,
            userName: request.UserName, 
            cancellationToken: cancellationToken);
        if (isExistByUserNameForUpdate) return Results.Conflict("Пользователь с данным именем уже существует!");

        var isExistByEmailForUpdate = await userRepository.IsExistByEmailForUpdateAsync(
            userId: userId,
            email: request.UserName, 
            cancellationToken: cancellationToken);
        if (isExistByEmailForUpdate) return Results.Conflict("Пользователь с данной почтой уже существует!");

        var user = await userRepository.GetByIdAsync(
            id: userId,
            cancellationToken: cancellationToken);
        if (user == null) return Results.NotFound("Пользователь не найден!");
        
        user.LastName = request.LastName;
        user.FirstName = request.FirstName;
        user.MiddleName = request.MiddleName;
        
        user.UserName = request.UserName;
        
        user.Email = request.Email;
        
        user.PasswordHash = request.IsChangingPassword 
            ? BCrypt.Net.BCrypt.HashPassword(request.NewPassword)
            : user.PasswordHash;
        
        await userRepository.UpdateAsync(user, cancellationToken);
        
        return Results.NoContent();
    }
}