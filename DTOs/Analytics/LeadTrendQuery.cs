namespace app.DTOs.Analytics;

public class LeadTrendQuery
{
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public string? GroupBy { get; set; } = "day";
}
