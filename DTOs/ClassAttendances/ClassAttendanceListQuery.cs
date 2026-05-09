using app.Common.Pagination;

namespace app.DTOs.ClassAttendances;

public class ClassAttendanceListQuery : PaginationQuery
{
    public long? ClassSessionId { get; set; }
    public long? ClassStudentId { get; set; }
    public bool? IsAbsent { get; set; }
    public long? RecordedBy { get; set; }
    public DateTime? RecordedFrom { get; set; }
    public DateTime? RecordedTo { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
