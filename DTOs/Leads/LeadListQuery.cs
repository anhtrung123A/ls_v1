using app.Common.Pagination;

namespace app.DTOs.Leads;

public class LeadListQuery : PaginationQuery
{
    public string? Keyword { get; set; }
    public byte? Source { get; set; }
    public byte? Status { get; set; }
    public long? AssignedTo { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
