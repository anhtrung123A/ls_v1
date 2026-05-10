namespace app.DTOs.Analytics;

public class AttendanceOverviewQuery
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
}
