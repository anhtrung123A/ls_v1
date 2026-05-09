using Microsoft.EntityFrameworkCore;
using app.Data.EF;
using app.DTOs.Users;

namespace app.Repositories.Users;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfileResponse?> GetUserProfileAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserProfileResponse
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role,
                AvatarUrl = u.AvatarUrl,
                IsActive = u.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserProfileResponse?> UpdateUserProfileAsync(long userId, UpdateUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        user.FullName = request.FullName?.Trim();
        user.Phone = request.Phone?.Trim();
        user.AvatarUrl = request.AvatarUrl?.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserProfileResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive
        };
    }
}
