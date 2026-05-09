namespace app.Data.EF.Entities;

public class Enrollment
{
    public long Id { get; set; }
    public long StudentId { get; set; }
    public long ClassId { get; set; }
    public byte Status { get; set; } = 1;
    public decimal? TuitionFee { get; set; }
    public decimal Discount { get; set; } = 0;
    public string? DiscountReason { get; set; }
    public decimal? FinalFee { get; set; }
    public long? EnrolledBy { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
}
