namespace app.DTOs.Payments;

public class CreatePaymentRequest
{
    public long InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public byte? Method { get; set; }
    public string? TransactionRef { get; set; }
    public string? Note { get; set; }
    public DateTime? PaidAt { get; set; }
}
