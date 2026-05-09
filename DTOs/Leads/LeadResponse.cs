namespace app.DTOs.Leads;

public class LeadResponse
{
    public long Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public byte? Source { get; init; }
    public string? Campaign { get; init; }
    public string? Interest { get; init; }
    public byte Status { get; init; }
    public string? LostReason { get; init; }
    public long? AssignedTo { get; init; }
    public long? ConvertedTo { get; init; }
    public string? Note { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? ConvertedAt { get; init; }
}
