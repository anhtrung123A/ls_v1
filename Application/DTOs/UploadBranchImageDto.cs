using Microsoft.AspNetCore.Http;

namespace app.Application.DTOs;

public class UploadBranchImageDto
{
    public IFormFile Image { get; set; } = default!;
}
