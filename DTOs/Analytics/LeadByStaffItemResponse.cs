namespace app.DTOs.Analytics;

public class LeadByStaffItemResponse
{
    public long StaffId { get; init; }
    public string StaffName { get; init; } = string.Empty;
    public long AssignedLeads { get; init; }
    public long ContactedLeads { get; init; }
    public long QualifiedLeads { get; init; }
    public long DemoScheduledLeads { get; init; }
    public long ConvertedLeads { get; init; }
    public long LostLeads { get; init; }
    public decimal ConversionRate { get; init; }
    public decimal LostRate { get; init; }
}
