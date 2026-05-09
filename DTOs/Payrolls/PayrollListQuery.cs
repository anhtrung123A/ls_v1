using app.Common.Pagination;

namespace app.DTOs.Payrolls;

public class PayrollListQuery : PaginationQuery
{
    public long? UserId { get; set; }
    public long? SalaryConfigId { get; set; }
    public byte? Month { get; set; }
    public int? Year { get; set; }
    public byte? Status { get; set; }
    public string? Keyword { get; set; }
    public DateTime? GeneratedFrom { get; set; }
    public DateTime? GeneratedTo { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
