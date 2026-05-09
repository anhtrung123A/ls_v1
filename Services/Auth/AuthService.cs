using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using app.Configurations;
using app.Data.EF;
using app.Data.EF.Entities;
using app.Data.Redis;
using app.DTOs.Auth;
using app.Models.Auth;

namespace app.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly RedisService _redisService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        AppDbContext dbContext,
        RedisService redisService,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _redisService = redisService;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var existed = await _dbContext.Users
            .AnyAsync(u => u.Email == email, cancellationToken);

        if (existed)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            FullName = request.FullName?.Trim(),
            Email = email,
            Phone = request.Phone?.Trim(),
            Role = request.Role ?? 4,
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsActive = true
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash) || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is inactive.");
        }

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var key = BuildRefreshTokenKey(request.RefreshToken);
        var session = await _redisService.GetAsync<RefreshTokenSession>(key);

        if (session is null || session.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token is invalid or expired.");
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == session.UserId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("User is not available.");
        }

        await _redisService.RemoveAsync(key);

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        await _redisService.RemoveAsync(BuildRefreshTokenKey(request.RefreshToken));
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(User user, CancellationToken cancellationToken)
    {
        var staff = await _dbContext.Staff
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == user.Id, cancellationToken);

        var (accessToken, accessTokenExpiresAt) = _jwtTokenService.GenerateAccessToken(
            user,
            staffId: staff?.Id,
            department: staff?.Department);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var refreshExpiry = TimeSpan.FromDays(_jwtOptions.RefreshTokenDays);

        var session = new RefreshTokenSession
        {
            UserId = user.Id,
            ExpiresAtUtc = DateTime.UtcNow.Add(refreshExpiry)
        };

        await _redisService.SetAsync(BuildRefreshTokenKey(refreshToken), session, refreshExpiry);

        return new AuthResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshToken = refreshToken
        };
    }

    private static string BuildRefreshTokenKey(string refreshToken) => $"auth:refresh:{refreshToken}";
}
