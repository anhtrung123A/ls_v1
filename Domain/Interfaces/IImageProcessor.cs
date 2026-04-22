using app.Domain.Models;

namespace app.Domain.Interfaces;

public interface IImageProcessor
{
    Task<ImageProcessResult> ResizeTo256Async(Stream input, CancellationToken cancellationToken = default);
}
