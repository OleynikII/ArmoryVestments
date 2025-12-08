namespace Utilities.Models;

public record PaginatedData<TResponse>(
    int TotalCount,
    IList<TResponse> Data,
    bool IsHaveNextPage,
    bool IsHavePrevPage);