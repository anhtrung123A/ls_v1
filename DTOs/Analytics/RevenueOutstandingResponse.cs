namespace app.DTOs.Analytics;

public class RevenueOutstandingResponse
{
    public decimal TotalOutstandingAmount { get; init; }
    public long TotalOutstandingInvoices { get; init; }
    public long TotalOverdueInvoices { get; init; }
    public required IReadOnlyList<RevenueOutstandingItemResponse> Items { get; init; }
    public required IReadOnlyList<RevenueOutstandingAgingItemResponse> Aging { get; init; }
}
