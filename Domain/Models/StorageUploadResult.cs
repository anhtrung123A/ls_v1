namespace app.Domain.Models;

public sealed class StorageUploadResult
{
    public required string BucketName { get; init; }
    public required string ObjectKey { get; init; }
    public required string PublicUrl { get; init; }
    public required string FileName { get; init; }
    public string? ContentType { get; init; }
    public required long Size { get; init; }
}
