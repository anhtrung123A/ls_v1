namespace app.DTOs.Analytics;

public class RevenueOutstandingItemResponse
{
    public long InvoiceId { get; init; }
    public string InvoiceNo { get; init; } = string.Empty;
    public long StudentId { get; init; }
    public string? StudentName { get; init; }
    public string? CourseName { get; init; }
    public decimal FinalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal RemainingAmount { get; init; }
    public DateOnly? DueDate { get; init; }
    public int DaysOverdue { get; init; }
    public string Status { get; init; } = "unpaid";
}
