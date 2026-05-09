namespace app.DTOs.Auth;

public class ChangePasswordRequest
{
    public required string Email { get; set; }
    public required string NewPassword { get; set; }
    public string? Otp { get; set; }
}
