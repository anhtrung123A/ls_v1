namespace app.DTOs.Analytics;

public class LeadTrendItemResponse
{
    public string Date { get; init; } = string.Empty;
    public long TotalLeads { get; init; }
    public long ConvertedLeads { get; init; }
    public long LostLeads { get; init; }
    public decimal ConversionRate { get; init; }
}
