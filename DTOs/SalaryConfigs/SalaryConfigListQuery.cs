using app.Common.Pagination;

namespace app.DTOs.SalaryConfigs;

public class SalaryConfigListQuery : PaginationQuery
{
    public long? UserId { get; set; }
    public byte? SalaryType { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
