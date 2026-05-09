namespace app.DTOs.Rooms;

public class RoomResponse
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Location { get; init; }
    public int? Capacity { get; init; }
    public string? Facilities { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
