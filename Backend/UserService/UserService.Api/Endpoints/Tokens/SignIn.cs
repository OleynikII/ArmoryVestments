namespace UserService.Api.Endpoints.Tokens;

public static class SignIn
{
    public record Request(string UserName, string Password);
    public record Response(string AccessToken, string RefreshToken);

    public sealed class Validator : AbstractValidator<Request>
    {
        private const string UsernameRegex = @"^[a-zA-Z0-9_]+$";
        
        public Validator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Имя пользователя обязательно для заполнения!")
                .MaximumLength(32).WithMessage("Длина логина не должна превышать 32 символов!")
                .Matches(UsernameRegex).WithMessage("Логин может содержать только латинские буквы, цифры и подчёркивания!")
                .Must(BeValidUsername).WithMessage("Логин не может состоять только из цифр!");
            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен при изменении пароля!")
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
    }
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("tokens", Handler)
                .WithTags("Tokens")
                .WithDescription("Login user")
                .Produces<Response>()
                .Produces<IList<string>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden);
        }
    }

    private static async Task<IResult> Handler(
        Request request,
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IValidator<Request> validator,
        IJwtHelper jwtHelper,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        
        var user = await userRepository.GetByUserNameAsync(
            userName: request.UserName, 
            include: u => u.Include(x => x.Roles).ThenInclude(x => x.Permissions), 
            cancellationToken: cancellationToken);
        if (user == null) return Results.Unauthorized();

        var isPasswordConfirmed = BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash);
        if (!isPasswordConfirmed) return Results.Unauthorized();
        
        if (!user.IsActive) return Results.Forbid();
        
        if (!user.IsEmailConfirmed) return Results.Forbid();
        
        var refreshToken = jwtHelper.GenerateRefreshToken();
        var accessToken = jwtHelper.GenerateJwtToken(user);

        var session = new Session(
            userId: user.Id,
            token: refreshToken,
            expiresAt: DateTime.UtcNow.AddDays(30));
        await sessionRepository.AddAsync(session, cancellationToken); 
        
        var response = new Response(
            AccessToken: accessToken,
            RefreshToken: refreshToken);
        
        return Results.Ok(response);
    }
    
    
}