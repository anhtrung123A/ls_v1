namespace app.Application.DTOs.Responses;

public class PaginatedResultDto<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int CurrentPage { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
}
