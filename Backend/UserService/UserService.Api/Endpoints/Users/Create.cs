namespace UserService.Api.Endpoints.Users;

public static class Create
{
    public record Request(
        string LastName,
        string FirstName,
        string? MiddleName,
        string UserName,
        string Email, 
        string Password,
        bool IsEmailConfirmed,
        bool ActivateUser);
    public record Response(
        Guid Id,
        string LastName, 
        string FirstName,
        string? MiddleName,
        string UserName,
        string Email,
        bool IsEmailConfirmed,
        bool IsActive);

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
                .MaximumLength(128).WithMessage("Длина почты не должна превышать 32 символов!");
             
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль не может быть пустым!")
                .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов!")
                .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву!")
                .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву!")
                .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру!");
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
            app.MapPost("users", Handler)
                .WithTags("Users")
                .WithDescription("Creating user")
                .Produces<Response>(StatusCodes.Status201Created)
                .Produces<IList<string>>(StatusCodes.Status400BadRequest)
                .Produces<string>(StatusCodes.Status409Conflict)
                .RequireAuthorization(Permissions.Users.Create);
        }
    }
    
    public static async Task<IResult> Handler(
        Request request,
        IUserRepository userRepository,
        IValidator<Request> validator,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        var isExistByUsername = await userRepository.IsExistByUserNameAsync(
            userName: request.UserName,
            cancellationToken: cancellationToken);
        if (isExistByUsername) return Results.Conflict("Пользователь с данным логином уже существует!");

          var isExistByEmail = await userRepository.IsExistByEmailAsync(
            email: request.Email, 
            cancellationToken: cancellationToken);
        if (isExistByEmail) return Results.Conflict("Пользователь с данной почтой уже существует!");
        
        var user = new User(
            lastName: request.LastName,
            firstName: request.FirstName,
            middleName: request.MiddleName, 
            userName: request.UserName,
            email: request.Email,
            passwordHash: BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password),
            isEmailConfirmed: request.IsEmailConfirmed,
            activateUser: request.ActivateUser);
        await userRepository.AddAsync(user, cancellationToken);
            
        var response = new Response(
            Id: user.Id, 
            LastName: user.LastName,
            FirstName: user.FirstName,
            MiddleName: user.MiddleName, 
            UserName: user.UserName,
            Email: user.Email, 
            IsEmailConfirmed: user.IsEmailConfirmed,
            IsActive: user.IsActive);
            
        return Results.Created($"users/{response.Id}", response);
    }
}