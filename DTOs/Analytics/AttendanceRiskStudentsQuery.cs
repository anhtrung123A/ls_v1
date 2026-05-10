namespace app.DTOs.Analytics;

public class AttendanceRiskStudentsQuery
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    public decimal Threshold { get; init; } = 70m;
}
