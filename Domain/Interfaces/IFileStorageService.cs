using app.Domain.Models;

namespace app.Domain.Interfaces;

public interface IFileStorageService
{
    Task<StorageUploadResult> UploadAsync(StorageUploadRequest request, CancellationToken cancellationToken = default);
}
