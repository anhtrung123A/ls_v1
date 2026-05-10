namespace app.DTOs.Analytics;

public class RevenueTrendQuery
{
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public string? GroupBy { get; set; } = "month";
}
