using Microsoft.AspNetCore.Http;

namespace app.Application.DTOs;

public class UploadUserAvatarDto
{
    public IFormFile Avatar { get; set; } = default!;
}
