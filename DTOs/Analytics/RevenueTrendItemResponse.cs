namespace app.DTOs.Analytics;

public class RevenueTrendItemResponse
{
    public string Period { get; init; } = string.Empty;
    public decimal Revenue { get; init; }
    public decimal PaidRevenue { get; init; }
    public decimal OutstandingRevenue { get; init; }
    public long InvoiceCount { get; init; }
}
