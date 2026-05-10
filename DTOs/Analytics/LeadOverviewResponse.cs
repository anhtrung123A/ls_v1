namespace app.DTOs.Analytics;

public class LeadOverviewResponse
{
    public long TotalLeads { get; init; }
    public long NewLeads { get; init; }
    public long ContactedLeads { get; init; }
    public long QualifiedLeads { get; init; }
    public long DemoScheduledLeads { get; init; }
    public long ConvertedLeads { get; init; }
    public long LostLeads { get; init; }
    public decimal ConversionRate { get; init; }
    public decimal LostRate { get; init; }
}
