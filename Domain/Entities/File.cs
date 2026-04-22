namespace app.Domain.Entities;

public class File
{
    public ulong Id { get; set; }
    public string ObjectKey { get; set; } = string.Empty;
    public string? BucketName { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long? Size { get; set; }
    public string? Checksum { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public ulong? CreatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? Metadata { get; set; }
}
