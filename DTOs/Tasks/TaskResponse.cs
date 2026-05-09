namespace app.DTOs.Tasks;

public class TaskResponse
{
    public long Id { get; init; }
    public long? AssignedTo { get; init; }
    public long? CreatedBy { get; init; }
    public long? RelatedLeadId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Type { get; init; }
    public byte Priority { get; init; }
    public byte Status { get; init; }
    public DateTime? DueAt { get; init; }
    public DateTime? DoneAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
