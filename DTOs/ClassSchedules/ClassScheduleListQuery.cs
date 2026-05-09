using app.Common.Pagination;

namespace app.DTOs.ClassSchedules;

public class ClassScheduleListQuery : PaginationQuery
{
    public long? ClassId { get; set; }
    public byte? Weekday { get; set; }
    public long? TeacherId { get; set; }
    public long? RoomId { get; set; }
    public bool? IsActive { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
