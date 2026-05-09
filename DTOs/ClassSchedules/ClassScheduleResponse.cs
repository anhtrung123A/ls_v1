namespace app.DTOs.ClassSchedules;

public class ClassScheduleResponse
{
    public long Id { get; init; }
    public long ClassId { get; init; }
    public long? RoomId { get; init; }
    public byte Weekday { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public DateTime CreatedAt { get; init; }
}
