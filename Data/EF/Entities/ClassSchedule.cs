namespace app.Data.EF.Entities;

public class ClassSchedule
{
    public long Id { get; set; }
    public long ClassId { get; set; }
    public long? RoomId { get; set; }
    // monday 1 ... sunday 7
    public byte Weekday { get; set; }
    public DateTime CreatedAt { get; set; }
}
