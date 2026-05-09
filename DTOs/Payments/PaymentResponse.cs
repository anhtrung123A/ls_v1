namespace app.DTOs.Payments;

public class PaymentResponse
{
    public long Id { get; init; }
    public long InvoiceId { get; init; }
    public decimal Amount { get; init; }
    public byte? Method { get; init; }
    public byte Status { get; init; }
    public string? TransactionRef { get; init; }
    public string? Note { get; init; }
    public long? CollectedBy { get; init; }
    public DateTime? PaidAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
