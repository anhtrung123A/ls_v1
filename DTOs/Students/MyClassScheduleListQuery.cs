using app.Common.Pagination;

namespace app.DTOs.Students;

public class MyClassScheduleListQuery : PaginationQuery
{
    public long? ClassId { get; set; }
    public byte? Weekday { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
