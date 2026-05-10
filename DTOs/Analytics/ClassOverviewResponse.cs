namespace app.DTOs.Analytics;

public class ClassOverviewResponse
{
    public long TotalClasses { get; init; }
    public long UpcomingClasses { get; init; }
    public long InProgressClasses { get; init; }
    public long FinishedClasses { get; init; }
    public long CancelledClasses { get; init; }
    public long TotalCapacity { get; init; }
    public long TotalStudents { get; init; }
    public decimal AverageFillRate { get; init; }
}
