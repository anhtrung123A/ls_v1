using app.Common.Pagination;

namespace app.DTOs.Students;

public class MyClassListQuery : PaginationQuery
{
    public string? Keyword { get; set; }
    public long? CourseId { get; set; }
    public byte? Status { get; set; }
    public byte? Type { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
