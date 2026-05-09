using app.Common.Pagination;

namespace app.DTOs.Courses;

public class CourseListQuery : PaginationQuery
{
    public string? Keyword { get; set; }
    public byte? Status { get; set; }
    public long? CategoryId { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
