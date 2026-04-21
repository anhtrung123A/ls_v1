namespace app.Application.DTOs.Auth;

public class CreateAuthUserRequestDto
{
    public string? LoginId { get; set; }
    public DateTime RegisteredAtUtc { get; set; }
    public string? RegisteredByEmail { get; set; }
    public string? UpdatedByEmail { get; set; }
}
