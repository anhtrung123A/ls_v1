namespace app.Data.EF.Entities;

public class ClassSession
{
    public long Id { get; set; }
    public long ClassId { get; set; }
    public DateOnly SessionDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public long? TeacherId { get; set; }
    public long? RoomId { get; set; }
    public byte Type { get; set; } = 1;
    public string? OnlineLink { get; set; }
    public byte Status { get; set; } = 1;
    public long? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
