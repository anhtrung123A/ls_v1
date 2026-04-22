using app.Application.DTOs;
using app.Application.DTOs.Responses;
using app.Application.UseCases;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly GetUserProfileUseCase _getUserProfileUseCase;

    public UsersController(
        CreateUserUseCase createUserUseCase,
        GetUserProfileUseCase getUserProfileUseCase)
    {
        _createUserUseCase = createUserUseCase;
        _getUserProfileUseCase = getUserProfileUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserDto dto,
        CancellationToken cancellationToken)
    {
        var user = await _createUserUseCase.ExecuteAsync(dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<UserDto>.Ok(user, "User created successfully."));
    }

    [Authorize]
    [HttpGet("user_profile")]
    public async Task<IActionResult> GetUserProfile(CancellationToken cancellationToken)
    {
        var profile = await _getUserProfileUseCase.ExecuteAsync(User, cancellationToken);
        return Ok(ApiResponse<UserProfileDto>.Ok(profile, "User profile fetched successfully."));
    }
}
