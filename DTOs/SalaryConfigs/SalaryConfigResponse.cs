namespace app.DTOs.SalaryConfigs;

public class SalaryConfigResponse
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public byte SalaryType { get; init; }
    public decimal BaseSalary { get; init; }
    public decimal TeachingRate { get; init; }
    public decimal ConvertedLeadRate { get; init; }
    public DateOnly EffectiveFrom { get; init; }
    public DateOnly? EffectiveTo { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
