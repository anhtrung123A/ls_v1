namespace app.Data.EF.Entities;

public class Payroll
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long? SalaryConfigId { get; set; }
    public byte Month { get; set; }
    public int Year { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal TeachingAmount { get; set; }
    public decimal KpiAmount { get; set; }
    public decimal BonusAmount { get; set; }
    public decimal DeductionAmount { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    // draft 1 | confirmed 2 | paid 3 | cancelled 4
    public byte Status { get; set; } = 1;
    public DateTime? GeneratedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Note { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
