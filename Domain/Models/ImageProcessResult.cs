namespace app.Domain.Models;

public sealed class ImageProcessResult
{
    public required Stream Content { get; init; }
    public required long Size { get; init; }
    public required string ContentType { get; init; }
    public required string FileExtension { get; init; }
}
