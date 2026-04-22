using System.Security.Claims;
using app.Application.Errors;
using app.Domain.Constants;

namespace app.Application.UseCases;

internal static class UserClaimResolver
{
    public static string GetEmailOrThrow(ClaimsPrincipal user)
    {
        var email = user.FindFirstValue(JwtClaimNames.Email)
            ?? user.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new UnauthorizedAccessException(AppErrors.Auth.TokenEmailMissing);
        }

        return email;
    }
}
