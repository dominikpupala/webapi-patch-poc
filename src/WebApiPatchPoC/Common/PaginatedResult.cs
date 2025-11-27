namespace WebApiPatchPoC.Common;

internal sealed record PaginatedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);
