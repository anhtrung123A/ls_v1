namespace app.DTOs.Analytics;

public class RevenuePaymentMethodsResponse
{
    public required IReadOnlyList<RevenuePaymentMethodItemResponse> Items { get; init; }
}
