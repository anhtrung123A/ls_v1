namespace app.DTOs.Analytics;

public class LeadBySourceItemResponse
{
    public byte? Source { get; init; }
    public string SourceName { get; init; } = "Unknown";
    public long TotalLeads { get; init; }
    public long ConvertedLeads { get; init; }
    public long LostLeads { get; init; }
    public decimal ConversionRate { get; init; }
    public decimal LostRate { get; init; }
}
