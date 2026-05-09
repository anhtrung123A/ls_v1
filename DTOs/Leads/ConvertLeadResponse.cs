namespace app.DTOs.Leads;

public class ConvertLeadResponse
{
    public long LeadId { get; init; }
    public long StudentId { get; init; }
    public byte LeadStatus { get; init; }
    public DateTime ConvertedAt { get; init; }
}
