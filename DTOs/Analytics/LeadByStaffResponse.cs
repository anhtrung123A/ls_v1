namespace app.DTOs.Analytics;

public class LeadByStaffResponse
{
    public required IReadOnlyList<LeadByStaffItemResponse> Items { get; init; }
}
