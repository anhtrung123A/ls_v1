using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.Data.Redis;
using app.DTOs.Auth;
using app.Services.Auth;
using app.Services.Email;

namespace app.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const int OtpExpireMinutes = 2;

    private readonly IAuthService _authService;
    private readonly RedisService _redisService;
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public AuthController(
        IAuthService authService,
        RedisService redisService,
        AppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
    {
        _authService = authService;
        _redisService = redisService;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(response, "Register successfully."));
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(response, "Login successfully."));
    }

    [HttpPost("refresh-token")]
    public async Task<IResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RefreshTokenAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(response, "Refresh token successfully."));
    }

    [HttpPost("logout")]
    public async Task<IResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Logout successfully."));
    }

    [HttpPost("otp/generate")]
    public async Task<IResult> GenerateOtp([FromBody] GenerateOtpRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var otpKey = BuildOtpKey(email);
        var existedOtp = await _redisService.GetAsync<string>(otpKey);

        if (!string.IsNullOrWhiteSpace(existedOtp))
        {
            throw new InvalidOperationException("OTP already exists. Please wait until it expires.");
        }

        var otpCode = RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
        await _redisService.SetAsync(otpKey, otpCode, TimeSpan.FromMinutes(OtpExpireMinutes));
        await _emailService.SendOtpAsync(email, otpCode, OtpExpireMinutes, cancellationToken);

        return Results.Ok(ApiResponse.Ok(new
        {
            ExpireMinutes = OtpExpireMinutes
        }, "Generate OTP successfully."));
    }

    [HttpPost("otp/expiry")]
    public async Task<IResult> CheckOtpExpiry([FromBody] CheckOtpExpiryRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var otpKey = BuildOtpKey(email);
        var ttl = await _redisService.GetTimeToLiveAsync(otpKey);

        return Results.Ok(ApiResponse.Ok(new
        {
            Email = email,
            Exists = ttl.HasValue,
            RemainingSeconds = ttl.HasValue ? Math.Max(0, (long)ttl.Value.TotalSeconds) : 0
        }, "Get OTP expiry successfully."));
    }

    [HttpPost("change-password")]
    public async Task<IResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        var validByOtp = await ValidateOtpAsync(email, request.Otp);
        var jwtUserId = await TryGetUserIdFromJwtAsync();
        var validByJwt = jwtUserId.HasValue && jwtUserId.Value == user.Id;

        if (!validByOtp && !validByJwt)
        {
            throw new UnauthorizedAccessException("Require valid OTP or JWT.");
        }

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (validByOtp)
        {
            await _redisService.RemoveAsync(BuildOtpKey(email));
        }

        return Results.Ok(ApiResponse.Ok(null, "Change password successfully."));
    }

    private async Task<bool> ValidateOtpAsync(string email, string? otp)
    {
        if (string.IsNullOrWhiteSpace(otp))
        {
            return false;
        }

        var savedOtp = await _redisService.GetAsync<string>(BuildOtpKey(email));
        return !string.IsNullOrWhiteSpace(savedOtp) && string.Equals(savedOtp, otp.Trim(), StringComparison.Ordinal);
    }

    private async Task<long?> TryGetUserIdFromJwtAsync()
    {
        var authResult = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (!authResult.Succeeded || authResult.Principal is null)
        {
            return null;
        }

        var subject = authResult.Principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? authResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return long.TryParse(subject, out var userId) ? userId : null;
    }

    private static string BuildOtpKey(string email) => $"auth:otp:{email}";
}
