namespace app.Data.EF.Entities;

public class ClassEntity
{
    public long Id { get; set; }
    public long CourseId { get; set; }
    public required string ClassCode { get; set; }
    public string? Name { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int MaxStudents { get; set; } = 20;
    public int CurrentCount { get; set; } = 0;
    // offline 1 | online 2 | hybrid 3
    public byte Type { get; set; } = 1;
    // upcoming 1 | in_progress 2 | finished 3 | cancelled 4
    public byte Status { get; set; } = 1;
    public long? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
