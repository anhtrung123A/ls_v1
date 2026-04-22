using app.Domain.Interfaces;
using app.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace app.Infrastructure.ExternalServices;

public class StorageFileUrlResolver : IFileUrlResolver
{
    private readonly StorageOptions _storageOptions;

    public StorageFileUrlResolver(IOptions<StorageOptions> storageOptions)
    {
        _storageOptions = storageOptions.Value;
    }

    public string? BuildPublicUrl(string? objectKey)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return null;
        }

        return $"{_storageOptions.PublicBaseUrl.TrimEnd('/')}/{objectKey}";
    }
}
