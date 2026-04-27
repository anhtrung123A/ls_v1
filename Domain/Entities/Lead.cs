namespace app.Domain.Entities;

public class Lead
{
    public ulong Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public byte Source { get; set; }
    public byte Status { get; set; }
    public ulong? AssignedTo { get; set; }
    public string? Note { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ulong? CreatedByUserId { get; set; }
    public ulong? UpdatedByUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
}
