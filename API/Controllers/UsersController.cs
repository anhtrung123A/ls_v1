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
    private readonly UpsertUserAvatarUseCase _upsertUserAvatarUseCase;
    private readonly DeleteUserAvatarUseCase _deleteUserAvatarUseCase;

    public UsersController(
        CreateUserUseCase createUserUseCase,
        GetUserProfileUseCase getUserProfileUseCase,
        UpsertUserAvatarUseCase upsertUserAvatarUseCase,
        DeleteUserAvatarUseCase deleteUserAvatarUseCase)
    {
        _createUserUseCase = createUserUseCase;
        _getUserProfileUseCase = getUserProfileUseCase;
        _upsertUserAvatarUseCase = upsertUserAvatarUseCase;
        _deleteUserAvatarUseCase = deleteUserAvatarUseCase;
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

    [Authorize]
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(
        [FromForm] UploadUserAvatarDto dto,
        CancellationToken cancellationToken)
    {
        var avatar = await _upsertUserAvatarUseCase.ExecuteAsync(User, dto.Avatar, cancellationToken);
        return Ok(ApiResponse<UserAvatarDto>.Ok(avatar, "User avatar uploaded successfully."));
    }

    [Authorize]
    [HttpPut("avatar")]
    public async Task<IActionResult> EditAvatar(
        [FromForm] UploadUserAvatarDto dto,
        CancellationToken cancellationToken)
    {
        var avatar = await _upsertUserAvatarUseCase.ExecuteAsync(User, dto.Avatar, cancellationToken);
        return Ok(ApiResponse<UserAvatarDto>.Ok(avatar, "User avatar updated successfully."));
    }

    [Authorize]
    [HttpDelete("avatar")]
    public async Task<IActionResult> DeleteAvatar(CancellationToken cancellationToken)
    {
        var result = await _deleteUserAvatarUseCase.ExecuteAsync(User, cancellationToken);
        return Ok(ApiResponse<DeleteUserAvatarResultDto>.Ok(result, "User avatar deleted successfully."));
    }
}
