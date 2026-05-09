namespace app.DTOs.Invoices;

public class CreateInvoiceRequest
{
    public long EnrollmentId { get; set; }
    public long StudentId { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? Note { get; set; }
}
