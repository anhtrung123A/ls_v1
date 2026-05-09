namespace app.DTOs.Students;

public class MyClassSessionResponse
{
    public long ClassSessionId { get; init; }
    public long ClassId { get; init; }
    public string? ClassCode { get; init; }
    public string? ClassName { get; init; }
    public DateOnly SessionDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public long? TeacherId { get; init; }
    public long? RoomId { get; init; }
    public string? RoomName { get; init; }
    public string? RoomLocation { get; init; }
    public byte Status { get; init; }
}
