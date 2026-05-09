namespace app.Data.EF.Entities;

public class SalaryConfig
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public byte SalaryType { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal TeachingRate { get; set; }
    public decimal ConvertedLeadRate { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
