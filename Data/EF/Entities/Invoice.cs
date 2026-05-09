namespace app.Data.EF.Entities;

public class Invoice
{
    public long Id { get; set; }
    public required string InvoiceNo { get; set; }
    public long EnrollmentId { get; set; }
    public long StudentId { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
    public decimal FinalAmount { get; set; }
    public decimal PaidAmount { get; set; } = 0;
    public decimal RemainingAmount { get; set; } = 0;
    public DateOnly? DueDate { get; set; }
    public byte Status { get; set; } = 1; // unpaid
    public string? Note { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
