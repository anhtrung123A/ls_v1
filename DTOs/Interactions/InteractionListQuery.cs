using app.Common.Pagination;

namespace app.DTOs.Interactions;

public class InteractionListQuery : PaginationQuery
{
    public long? LeadId { get; set; }
    public long? StaffId { get; set; }
    public string? Channel { get; set; }
    public string? Direction { get; set; }
    public string? Keyword { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
