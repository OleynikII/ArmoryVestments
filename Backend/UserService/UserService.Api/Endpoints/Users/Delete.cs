namespace UserService.Api.Endpoints.Users;

public static class Delete
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("users/{userId:guid}", Handler)
                .WithTags("Users")
                .WithDescription("Delete user by id")
                .Produces(StatusCodes.Status204NoContent)
                .Produces<IList<string>>(StatusCodes.Status400BadRequest)
                .Produces<string>(StatusCodes.Status404NotFound)
                .RequireAuthorization(Permissions.Users.Delete);
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
        
        await userRepository.DeleteAsync(user, cancellationToken);
        
        return Results.NoContent();
    }
}