namespace UserService.Api.Endpoints.Users;

public static class Delete
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("users/{userId:guid}", Handler)
                .WithTags("Users");
        }
    }
    
    public static async Task<IResult> Handler(
        Guid userId,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty) return Results.BadRequest("Некорректный формат идентификатора!");

        var user = await userRepository.GetByIdAsync(
            id: userId,
            cancellationToken: cancellationToken);
        if (user == null) return Results.NotFound("Пользователь не найден!");
        
        await userRepository.DeleteAsync(user, cancellationToken);
        
        return Results.NoContent();
    }
}