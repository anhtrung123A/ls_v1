namespace app.Application.DTOs.Auth;

public class CreateAuthUserResponseDto
{
    public bool Success { get; set; }
    public int Status { get; set; }
    public CreateAuthUserDataDto? Data { get; set; }
}

public class CreateAuthUserDataDto
{
    public string Id { get; set; } = string.Empty;
    public string? LoginId { get; set; }
    public DateTime RegisteredAtUtc { get; set; }
    public string? RegisteredByEmail { get; set; }
    public string? UpdatedByEmail { get; set; }
    public string? GeneratedPassword { get; set; }
}
