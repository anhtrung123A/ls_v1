namespace app.Data.EF.Entities;

public class BatchJobExecution
{
    public long Id { get; set; }
    public required string JobName { get; set; }
    // running 1 / success 2 / failed 3 / skipped 4
    public byte Status { get; set; }
    // cron 1 / manual 2 / system 3
    public byte? TriggeredBy { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public long? DurationMs { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorTrace { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
