namespace app.Data.EF.Entities;

public class BatchJobLock
{
    public required string JobName { get; set; }
    public DateTime LockedAt { get; set; }
    public required string LockedBy { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
