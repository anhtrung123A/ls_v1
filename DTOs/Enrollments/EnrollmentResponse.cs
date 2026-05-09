namespace app.DTOs.Enrollments;

public class EnrollmentResponse
{
    public long Id { get; init; }
    public string? StudentName { get; init; }
    public string? ClassName { get; init; }
    public byte Status { get; init; }
    public decimal? TuitionFee { get; init; }
    public decimal Discount { get; init; }
    public string? DiscountReason { get; init; }
    public decimal? FinalFee { get; init; }
    public long? EnrolledBy { get; init; }
    public DateTime EnrolledAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? Notes { get; init; }
}
