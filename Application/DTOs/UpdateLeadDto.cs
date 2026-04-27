using app.Domain.Enums;

namespace app.Application.DTOs;

public class UpdateLeadDto
{
    public string FirstName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public LeadSource Source { get; set; }
    public LeadStatus Status { get; set; }
    public string? Phonenumber { get; set; }
    public ulong? AssignedTo { get; set; }
    public string? Note { get; set; }
    public string? Metadata { get; set; }
}
