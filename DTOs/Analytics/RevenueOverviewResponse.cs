namespace app.DTOs.Analytics;

public class RevenueOverviewResponse
{
    public decimal TotalRevenue { get; init; }
    public decimal PaidRevenue { get; init; }
    public decimal OutstandingRevenue { get; init; }
    public long TotalInvoices { get; init; }
    public long PaidInvoices { get; init; }
    public long OverdueInvoices { get; init; }
}
