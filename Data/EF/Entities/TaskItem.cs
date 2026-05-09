namespace app.Data.EF.Entities;

public class TaskItem
{
    public long Id { get; set; }
    public long? AssignedTo { get; set; }
    public long? CreatedBy { get; set; }
    public long? RelatedLeadId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    // low 1 | medium 2 | high 3
    public byte Priority { get; set; } = 2;
    // open 1 | closed 2
    public byte Status { get; set; } = 1;
    public DateTime? DueAt { get; set; }
    public DateTime? DoneAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
