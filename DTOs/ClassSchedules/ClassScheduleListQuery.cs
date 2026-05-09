using app.Common.Pagination;

namespace app.DTOs.ClassSchedules;

public class ClassScheduleListQuery : PaginationQuery
{
    public long? ClassId { get; set; }
    public byte? Weekday { get; set; }
    public long? RoomId { get; set; }
    public TimeOnly? StartTimeFrom { get; set; }
    public TimeOnly? StartTimeTo { get; set; }
    public TimeOnly? EndTimeFrom { get; set; }
    public TimeOnly? EndTimeTo { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
