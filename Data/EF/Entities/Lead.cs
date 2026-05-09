namespace app.Data.EF.Entities;

public class Lead
{
    public long Id { get; set; }
    public required string FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    // facebook 1 | zalo 2 | referral 3 | walk_in 4 | website 5
    public byte? Source { get; set; }
    public string? Campaign { get; set; }
    public string? Interest { get; set; }
    // new 1 | contacted 2 | qualified 3 | demo_scheduled 4 | converted 5 | lost 6
    public byte Status { get; set; } = 1;
    public string? LostReason { get; set; }
    public long? AssignedTo { get; set; }
    public long? ConvertedTo { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ConvertedAt { get; set; }
}
