using app.Common.Pagination;

namespace app.DTOs.Enrollments;

public class EnrollmentListQuery : PaginationQuery
{
    public string? Keyword { get; set; }
    public byte? Status { get; set; }
    public long? EnrolledBy { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
