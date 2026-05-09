namespace app.DTOs.Payrolls;

public class PayrollResponse
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public string? UserFullName { get; init; }
    public string? UserEmail { get; init; }
    public long? SalaryConfigId { get; init; }
    public byte Month { get; init; }
    public int Year { get; init; }
    public decimal BaseAmount { get; init; }
    public decimal TeachingAmount { get; init; }
    public decimal KpiAmount { get; init; }
    public decimal BonusAmount { get; init; }
    public decimal DeductionAmount { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal NetAmount { get; init; }
    public byte Status { get; init; }
    public DateTime? GeneratedAt { get; init; }
    public DateTime? ConfirmedAt { get; init; }
    public DateTime? PaidAt { get; init; }
    public string? Note { get; init; }
    public long? CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
