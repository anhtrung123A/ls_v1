using app.Common.Pagination;

namespace app.DTOs.Students;

public class StudentListQuery : PaginationQuery
{
    public string? Keyword { get; set; }
    public byte? Status { get; set; }
    public byte? Source { get; set; }
    public long? AssignedStaffId { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
