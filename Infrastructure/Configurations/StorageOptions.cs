using System.ComponentModel.DataAnnotations;

namespace app.Infrastructure.Configurations;

public class StorageOptions
{
    public const string SectionName = "Storage";

    [Required]
    public string Provider { get; set; } = string.Empty;

    [Required]
    public string Endpoint { get; set; } = string.Empty;

    [Required]
    public string AccessKey { get; set; } = string.Empty;

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string Bucket { get; set; } = string.Empty;

    public bool UseSsl { get; set; }

    [Required]
    public string PublicBaseUrl { get; set; } = string.Empty;
}
