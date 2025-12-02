namespace UserService.Api.Endpoints.Users;

public static class GetPaginated
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
            app.MapGet("users", Handler)
                .WithTags("Users")
                .WithDescription("Get paginated users")
                .Produces<PaginatedData<Response>>()
                .RequireAuthorization(Permissions.Users.Get);
        }
    }
    
    public static async Task<IResult> Handler(
        int pageNumber, int pageSize,
        string? searchTerm,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        var (totalCount, users, isHaveNextPage, isHavePrevPage) = await userRepository.GetPaginatedAsync(
            pageNumber: pageNumber,
            pageSize: pageSize, 
            orderBy: users => users.OrderByDescending(x => x.CreatedAt),
            predicate: !string.IsNullOrWhiteSpace(searchTerm) ? users => users.Email.Contains(searchTerm) : null,
            cancellationToken: cancellationToken);
        
        var usersResponse = users.Select(user => 
            new Response(
                Id: user.Id, 
                LastName: user.LastName, 
                FirstName: user.FirstName, 
                MiddleName: user.MiddleName,
                UserName: user.UserName,
                Email: user.Email, 
                IsEmailConfirmed: user.IsEmailConfirmed,
                user.IsActive)).ToList();

        var paginatedResponse = new PaginatedData<Response>(
            TotalCount: totalCount,
            Data: usersResponse,
            IsHaveNextPage: isHaveNextPage,
            IsHavePrevPage: isHavePrevPage);
        
        return Results.Ok(paginatedResponse);
    }
}