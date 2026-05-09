using System.ComponentModel.DataAnnotations;

namespace app.DTOs.Staff;

public class CreateStaffRequest
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Range(1, 4)]
    public byte Department { get; set; }

    [MaxLength(100)]
    public string? Position { get; set; }

    public long? ManagerId { get; set; }
}
