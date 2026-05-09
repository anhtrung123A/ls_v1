namespace app.Data.EF.Entities;

public class Interaction
{
    public long Id { get; set; }
    public long? LeadId { get; set; }
    public long? StaffId { get; set; }
    public string? Channel { get; set; }
    public string? Direction { get; set; }
    public string? Content { get; set; }
    public string? Outcome { get; set; }
    public string? Attachments { get; set; }
    public DateTime OccurredAt { get; set; }
}
