namespace app.DTOs.Interactions;

public class InteractionResponse
{
    public long Id { get; init; }
    public long? LeadId { get; init; }
    public long? StaffId { get; init; }
    public string? Channel { get; init; }
    public string? Direction { get; init; }
    public string? Content { get; init; }
    public string? Outcome { get; init; }
    public string? Attachments { get; init; }
    public DateTime OccurredAt { get; init; }
}
