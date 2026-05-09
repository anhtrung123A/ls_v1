namespace app.DTOs.ClassSchedules;

public class CreateClassScheduleRequest
{
    public long ClassId { get; set; }
    public long? RoomId { get; set; }
    public byte Weekday { get; set; }
}
