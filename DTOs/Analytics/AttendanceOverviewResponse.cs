namespace app.DTOs.Analytics;

public class AttendanceOverviewResponse
{
    public long TotalSessions { get; init; }
    public long CompletedSessions { get; init; }
    public long TotalAttendanceRecords { get; init; }
    public long PresentCount { get; init; }
    public long AbsentCount { get; init; }
    public decimal AttendanceRate { get; init; }
    public decimal AbsentRate { get; init; }
}
