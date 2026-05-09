namespace app.DTOs.Leads;

public class UpdateLeadRequest
{
    public required string FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public byte? Source { get; set; }
    public string? Campaign { get; set; }
    public string? Interest { get; set; }
    public byte Status { get; set; }
    public string? LostReason { get; set; }
    public long? AssignedTo { get; set; }
    public long? ConvertedTo { get; set; }
    public string? Note { get; set; }
    public DateTime? ConvertedAt { get; set; }
}
