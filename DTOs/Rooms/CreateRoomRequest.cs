namespace app.DTOs.Rooms;

public class CreateRoomRequest
{
    public required string Name { get; set; }
    public string? Location { get; set; }
    public int? Capacity { get; set; }
    public string? Facilities { get; set; }
}
