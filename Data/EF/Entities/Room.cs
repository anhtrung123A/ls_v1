namespace app.Data.EF.Entities;

public class Room
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Location { get; set; }
    public int? Capacity { get; set; }
    public string? Facilities { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
