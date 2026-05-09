namespace app.DTOs.Invoices;

public class InvoiceResponse
{
    public long Id { get; init; }
    public string InvoiceNo { get; init; } = string.Empty;
    public long EnrollmentId { get; init; }
    public long StudentId { get; init; }
    public decimal SubtotalAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal FinalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal RemainingAmount { get; init; }
    public DateOnly? DueDate { get; init; }
    public byte Status { get; init; }
    public string? Note { get; init; }
    public long? CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
