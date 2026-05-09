namespace app.DTOs.ClassSchedules;

public class CreateClassScheduleRequest
{
    public long ClassId { get; set; }
    public long? TeacherId { get; set; }
    public long? RoomId { get; set; }
    public byte Weekday { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? OnlineLink { get; set; }
    public byte? Type { get; set; }
}
