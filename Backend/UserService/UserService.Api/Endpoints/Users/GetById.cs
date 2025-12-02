namespace UserService.Api.Endpoints.Users;

public static class GetById
{
    public record Response(
        Guid Id,
        string LastName,
        string FirstName,
        string? MiddleName,
        string UserName,
        string Email, 
        bool IsEmailConfirmed,
        bool IsActive);
      
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("users/{userId:guid}", Handler)
                .WithTags("Users")
                .WithDescription("Get user by id")
                .Produces<Response>()
                .Produces<IList<string>>(StatusCodes.Status400BadRequest)
                .Produces<string>(StatusCodes.Status404NotFound)
                .RequireAuthorization(Permissions.Users.Get);
        }
    }
    
    public static async Task<IResult> Handler(
        Guid userId,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty) 
            return Results.BadRequest(new List<string> { "Некорректный формат идентификатора!" });
        
        var user = await userRepository.GetByIdAsync(
            id: userId,
            cancellationToken: cancellationToken);
        if (user == null) return Results.NotFound("Пользователь не найден!");
        
        var userResponse = new Response(
            Id: user.Id, 
            LastName: user.LastName,
            FirstName: user.FirstName,
            MiddleName: user.MiddleName,
            UserName: user.UserName,
            Email: user.Email, 
            IsEmailConfirmed: user.IsEmailConfirmed,
            IsActive: user.IsActive);
        
        return Results.Ok(userResponse);
    }
}