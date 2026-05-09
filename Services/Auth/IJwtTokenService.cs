using app.Data.EF.Entities;

namespace app.Services.Auth;

public interface IJwtTokenService
{
    (string token, DateTime expiresAtUtc) GenerateAccessToken(User user, long? staffId = null, byte? department = null);
    string GenerateRefreshToken();
}
