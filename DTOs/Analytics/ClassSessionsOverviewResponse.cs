namespace app.DTOs.Analytics;

public class ClassSessionsOverviewResponse
{
    public long TotalSessions { get; init; }
    public long ScheduledSessions { get; init; }
    public long CompletedSessions { get; init; }
    public long CancelledSessions { get; init; }
    public long PostponedSessions { get; init; }
    public long RescheduledSessions { get; init; }
    public decimal CompletionRate { get; init; }
    public decimal CancelRate { get; init; }
}
