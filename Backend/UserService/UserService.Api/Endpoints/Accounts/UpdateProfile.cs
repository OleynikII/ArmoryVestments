namespace UserService.Api.Endpoints.Accounts;

public static class UpdateProfile
{
    public record Request(
        string LastName,
        string FirstName,
        string MiddleName,
        string UserName,
        string Email);

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
                .Matches(UsernameRegex)
                .WithMessage("Логин может содержать только латинские буквы, цифры и подчёркивания!")
                .Must(BeValidUsername).WithMessage("Логин не может состоять только из цифр!");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Почта не может быть пустой!")
                .EmailAddress().WithMessage("Некорректный формат почты!")
                .MaximumLength(128).WithMessage("Длина почты не должна превышать 32 символов!!");
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
            app.MapPut("accounts/update-profile", Handler)
                .WithTags("Accounts")
                .WithDescription("Update authenticated user")
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

        if (user.IsEmailConfirmed) user.IsEmailConfirmed = request.Email == user.Email;
        
        user.LastName = request.LastName;
        user.FirstName = request.FirstName;
        user.MiddleName = request.MiddleName;
        
        user.UserName = request.UserName;
        user.Email = request.Email;
        
        await userRepository.UpdateAsync(user, cancellationToken);
        
        return Results.NoContent();
    }

}