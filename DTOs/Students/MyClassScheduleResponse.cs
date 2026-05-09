namespace app.DTOs.Students;

public class MyClassScheduleResponse
{
    public long ClassScheduleId { get; init; }
    public long ClassId { get; init; }
    public string? ClassCode { get; init; }
    public string? ClassName { get; init; }
    public byte Weekday { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public long? RoomId { get; init; }
    public string? RoomName { get; init; }
    public string? RoomLocation { get; init; }
    public DateTime CreatedAt { get; init; }
}
