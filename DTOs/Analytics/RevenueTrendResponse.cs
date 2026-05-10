namespace app.DTOs.Analytics;

public class RevenueTrendResponse
{
    public string GroupBy { get; init; } = "month";
    public required IReadOnlyList<RevenueTrendItemResponse> Items { get; init; }
}
