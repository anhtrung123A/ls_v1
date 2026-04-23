namespace app.Application.DTOs;

public class BranchImageDto
{
    public ulong Id { get; set; }
    public string ObjectKey { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? Size { get; set; }
    public DateTime CreatedAt { get; set; }
}
