namespace app.DTOs.Enrollments;

public class CreateEnrollmentRequest
{
    public long StudentId { get; set; }
    public long ClassId { get; set; }
    public decimal? TuitionFee { get; set; }
    public decimal? Discount { get; set; }
    public string? DiscountReason { get; set; }
    public decimal? FinalFee { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
}
