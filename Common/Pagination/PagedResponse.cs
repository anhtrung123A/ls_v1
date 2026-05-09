namespace app.Common.Pagination;

public class PagedResponse<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required long TotalItems { get; init; }
    public required int TotalPages { get; init; }
}
