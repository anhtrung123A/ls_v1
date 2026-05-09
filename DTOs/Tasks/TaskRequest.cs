namespace app.DTOs.Tasks;

public class TaskRequest
{
    public long? AssignedTo { get; set; }
    public long? CreatedBy { get; set; }
    public long? RelatedLeadId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public byte? Priority { get; set; }
    public byte? Status { get; set; }
    public DateTime? DueAt { get; set; }
    public DateTime? DoneAt { get; set; }
}
