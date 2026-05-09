namespace app.Data.EF.Entities;

public class BatchJobItem
{
    public long Id { get; set; }
    public long ExecutionId { get; set; }
    public string? TargetType { get; set; }
    public long? TargetId { get; set; }
    // success 1 / failed 2 / skipped 3
    public byte Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
