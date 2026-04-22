using app.Domain.Interfaces;
using app.Domain.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace app.Infrastructure.ExternalServices;

public class ImageSharpProcessor : IImageProcessor
{
    private const int AvatarSizePx = 256;

    public async Task<ImageProcessResult> ResizeTo256Async(Stream input, CancellationToken cancellationToken = default)
    {
        using var image = await Image.LoadAsync(input, cancellationToken);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(AvatarSizePx, AvatarSizePx),
            Mode = ResizeMode.Max
        }));

        var output = new MemoryStream();
        await image.SaveAsPngAsync(output, cancellationToken);
        output.Position = 0;

        return new ImageProcessResult
        {
            Content = output,
            Size = output.Length,
            ContentType = "image/png",
            FileExtension = "png"
        };
    }
}
