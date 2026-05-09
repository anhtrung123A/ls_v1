using app.Common.Pagination;

namespace app.DTOs.Tasks;

public class TaskListQuery : PaginationQuery
{
    public long? RelatedLeadId { get; set; }
    public long? AssignedTo { get; set; }
    public long? CreatedBy { get; set; }
    public byte? Priority { get; set; }
    public byte? Status { get; set; }
    public string? Keyword { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
