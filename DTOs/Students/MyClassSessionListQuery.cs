using app.Common.Pagination;

namespace app.DTOs.Students;

public class MyClassSessionListQuery : PaginationQuery
{
    public long? ClassId { get; set; }
    public DateOnly? SessionDateFrom { get; set; }
    public DateOnly? SessionDateTo { get; set; }
    public byte? Status { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
