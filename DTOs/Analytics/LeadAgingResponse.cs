namespace app.DTOs.Analytics;

public class LeadAgingResponse
{
    public required IReadOnlyList<LeadAgingItemResponse> Items { get; init; }
    public long StaleLeads { get; init; }
}
