namespace app.Domain.Entities;

public class Branch
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public ulong? ImageFileId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ulong? CreatedByUserId { get; set; }
    public ulong? UpdatedByUserId { get; set; }
    public DateTime? DeletedAt { get; set; }
}
