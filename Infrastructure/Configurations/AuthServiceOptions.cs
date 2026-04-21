using System.ComponentModel.DataAnnotations;

namespace app.Infrastructure.Configurations;

public class AuthServiceOptions
{
    public const string SectionName = "Auth";

    [Required]
    public string BaseUrl { get; set; } = string.Empty;
}
