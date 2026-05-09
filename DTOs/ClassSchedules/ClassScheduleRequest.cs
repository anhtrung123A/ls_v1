namespace app.DTOs.ClassSchedules;

public class ClassScheduleRequest
{
    public long ClassId { get; set; }
    public long? RoomId { get; set; }
    public byte Weekday { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
