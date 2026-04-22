using System.ComponentModel.DataAnnotations;

namespace app.Infrastructure.Configurations;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Secret { get; set; } = string.Empty;
}
