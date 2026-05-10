namespace app.DTOs.Analytics;

public class TeacherWorkloadItemResponse
{
    public long TeacherId { get; init; }
    public string? TeacherName { get; init; }
    public long ClassCount { get; init; }
    public long SessionCount { get; init; }
    public decimal TotalTeachingHours { get; init; }
    public long CompletedSessions { get; init; }
    public long CancelledSessions { get; init; }
    public decimal AverageAttendanceRate { get; init; }
}
