namespace app.Domain.Models;

public sealed class StorageUploadRequest
{
    public required Stream Content { get; init; }
    public required long Size { get; init; }
    public required string FileName { get; init; }
    public string? ContentType { get; init; }
    public string? Folder { get; init; }
}
