namespace app.DTOs.Analytics;

public class ClassByCourseItemResponse
{
    public long CourseId { get; init; }
    public string CourseCode { get; init; } = string.Empty;
    public string CourseName { get; init; } = string.Empty;
    public long ClassCount { get; init; }
    public long StudentCount { get; init; }
    public long TotalCapacity { get; init; }
    public decimal AverageFillRate { get; init; }
    public long CompletedClasses { get; init; }
    public long InProgressClasses { get; init; }
    public long UpcomingClasses { get; init; }
}
