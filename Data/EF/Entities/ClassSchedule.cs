namespace app.Data.EF.Entities;

public class ClassSchedule
{
    public long Id { get; set; }
    public long ClassId { get; set; }
    public long? TeacherId { get; set; }
    public long? RoomId { get; set; }
    // monday 1 ... sunday 7
    public byte Weekday { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? OnlineLink { get; set; }
    // offline 1 | online 2
    public byte Type { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
