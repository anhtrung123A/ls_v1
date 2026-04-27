using app.Domain.Enums;

namespace app.Domain.Entities;

public class Lead
{
    public ulong Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public LeadSource Source { get; set; }
    public LeadStatus Status { get; set; }
    public string? Phonenumber { get; set; }
    public ulong? AssignedTo { get; set; }
    public string? Note { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ulong? CreatedByUserId { get; set; }
    public ulong? UpdatedByUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
}
