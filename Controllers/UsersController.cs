using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using app.Common.Responses;
using app.DTOs.Users;
using app.Repositories.Users;

namespace app.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("user_profile")]
    public async Task<IResult> UserProfile(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            throw new UnauthorizedAccessException("Invalid access token.");
        }

        var profile = await _userRepository.GetUserProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        return Results.Ok(ApiResponse.Ok(profile, "Get profile successfully."));
    }

    [HttpPut("user_profile")]
    public async Task<IResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            throw new UnauthorizedAccessException("Invalid access token.");
        }

        var profile = await _userRepository.UpdateUserProfileAsync(userId, request, cancellationToken);
        if (profile is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        return Results.Ok(ApiResponse.Ok(profile, "Update profile successfully."));
    }

    private bool TryGetUserId(out long userId)
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return long.TryParse(subject, out userId);
    }
}
