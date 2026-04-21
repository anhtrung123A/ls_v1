using app.Application.DTOs;
using app.Application.DTOs.Responses;
using app.Application.UseCases;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace app.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly CreateUserUseCase _useCase;

    public UsersController(CreateUserUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserDto dto,
        CancellationToken cancellationToken)
    {
        var user = await _useCase.ExecuteAsync(dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<UserDto>.Ok(user, "User created successfully."));
    }
}
