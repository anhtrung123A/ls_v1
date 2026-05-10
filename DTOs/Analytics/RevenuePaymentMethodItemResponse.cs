namespace app.DTOs.Analytics;

public class RevenuePaymentMethodItemResponse
{
    public byte? Method { get; init; }
    public string MethodName { get; init; } = "Unknown";
    public long PaymentCount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal Percentage { get; init; }
}
