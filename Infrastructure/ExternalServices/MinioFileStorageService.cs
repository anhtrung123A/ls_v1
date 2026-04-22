using app.Domain.Interfaces;
using app.Domain.Models;
using app.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System.Security.Cryptography;
using System.Text;

namespace app.Infrastructure.ExternalServices;

public class MinioFileStorageService : IFileStorageService
{
    private readonly StorageOptions _options;
    private readonly IMinioClient _minioClient;

    public MinioFileStorageService(IOptions<StorageOptions> options)
    {
        _options = options.Value;

        var endpointUri = new Uri(_options.Endpoint);
        var endpointHost = endpointUri.Host;
        var endpointPort = endpointUri.IsDefaultPort
            ? (endpointUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? 443 : 80)
            : endpointUri.Port;

        _minioClient = new MinioClient()
            .WithEndpoint(endpointHost, endpointPort)
            .WithCredentials(_options.AccessKey, _options.SecretKey)
            .WithSSL(_options.UseSsl)
            .Build();
    }

    public async Task<StorageUploadResult> UploadAsync(StorageUploadRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureBucketExistsAsync(cancellationToken);

        var objectKey = BuildObjectKey(request.Folder, request.FileName);
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectKey)
            .WithStreamData(request.Content)
            .WithObjectSize(request.Size)
            .WithContentType(request.ContentType ?? "application/octet-stream");

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

        return new StorageUploadResult
        {
            BucketName = _options.Bucket,
            ObjectKey = objectKey,
            PublicUrl = BuildPublicUrl(objectKey),
            FileName = request.FileName,
            ContentType = request.ContentType,
            Size = request.Size
        };
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        await _minioClient.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(_options.Bucket)
                .WithObject(objectKey),
            cancellationToken);
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        var bucketExists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.Bucket),
            cancellationToken);

        if (bucketExists)
        {
            return;
        }

        await _minioClient.MakeBucketAsync(
            new MakeBucketArgs().WithBucket(_options.Bucket),
            cancellationToken);
    }

    private string BuildPublicUrl(string objectKey)
    {
        var baseUrl = _options.PublicBaseUrl.TrimEnd('/');
        return $"{baseUrl}/{objectKey}";
    }

    private static string BuildObjectKey(string? folder, string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        var fileExtension = Path.GetExtension(safeFileName);
        var hashedFileName = HashFileName(safeFileName);
        var prefix = string.IsNullOrWhiteSpace(folder)
            ? string.Empty
            : $"{folder.Trim().Trim('/').Replace('\\', '/')}/";

        return $"{prefix}{hashedFileName}{fileExtension}";
    }

    private static string HashFileName(string fileName)
    {
        var normalizedName = fileName.Trim().ToLowerInvariant();
        var entropy = $"{normalizedName}|{Guid.NewGuid():N}|{DateTime.UtcNow.Ticks}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(entropy));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
