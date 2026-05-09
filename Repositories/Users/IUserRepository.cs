using app.DTOs.Users;

namespace app.Repositories.Users;

public interface IUserRepository
{
    Task<UserProfileResponse?> GetUserProfileAsync(long userId, CancellationToken cancellationToken = default);
    Task<UserProfileResponse?> UpdateUserProfileAsync(long userId, UpdateUserProfileRequest request, CancellationToken cancellationToken = default);
}
