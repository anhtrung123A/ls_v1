namespace app.DTOs.Analytics;

public class LeadBySourceResponse
{
    public required IReadOnlyList<LeadBySourceItemResponse> Items { get; init; }
}
