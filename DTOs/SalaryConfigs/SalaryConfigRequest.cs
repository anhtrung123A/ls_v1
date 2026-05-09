namespace app.DTOs.SalaryConfigs;

public class SalaryConfigRequest
{
    public long UserId { get; set; }
    public byte SalaryType { get; set; }
    public decimal? BaseSalary { get; set; }
    public decimal? TeachingRate { get; set; }
    public decimal? ConvertedLeadRate { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
}
