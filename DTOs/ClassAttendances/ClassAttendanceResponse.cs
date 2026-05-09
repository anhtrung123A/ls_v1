namespace app.DTOs.ClassAttendances;

public class ClassAttendanceResponse
{
    public long Id { get; init; }
    public long ClassSessionId { get; init; }
    public long ClassStudentId { get; init; }
    public bool? IsAbsent { get; init; }
    public string? AbsentReason { get; init; }
    public long? RecordedBy { get; init; }
    public DateTime RecordedAt { get; init; }
}
