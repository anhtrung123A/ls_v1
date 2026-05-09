namespace app.DTOs.Rooms;

public class RoomRequest
{
    public required string Name { get; set; }
    public string? Location { get; set; }
    public int? Capacity { get; set; }
    public string? Facilities { get; set; }
    public bool? IsActive { get; set; }
}
