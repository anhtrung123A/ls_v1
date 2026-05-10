namespace app.DTOs.Analytics;

public class LeadTrendResponse
{
    public string GroupBy { get; init; } = "day";
    public required IReadOnlyList<LeadTrendItemResponse> Items { get; init; }
}
