using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetUserProfileUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IFileUrlResolver _fileUrlResolver;

    public GetUserProfileUseCase(
        IUserRepository userRepository,
        IFileUrlResolver fileUrlResolver)
    {
        _userRepository = userRepository;
        _fileUrlResolver = fileUrlResolver;
    }

    public async Task<UserProfileDto> ExecuteAsync(
        System.Security.Claims.ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var email = UserClaimResolver.GetEmailOrThrow(user);

        var userWithAvatar = await _userRepository.GetWithAvatarByEmailAsync(email, cancellationToken);
        if (userWithAvatar.User is null)
        {
            throw new KeyNotFoundException(AppErrors.User.UserNotFoundByEmail);
        }

        var roleId = await _userRepository.GetRoleIdByUserIdAsync(userWithAvatar.User.Id, cancellationToken);

        return new UserProfileDto
        {
            Id = userWithAvatar.User.Id,
            RoleId = roleId,
            Firstname = userWithAvatar.User.Firstname,
            Lastname = userWithAvatar.User.Lastname,
            Email = userWithAvatar.User.Email,
            AvatarUrl = _fileUrlResolver.BuildPublicUrl(userWithAvatar.Avatar?.ObjectKey),
            Phonenumber = userWithAvatar.User.Phonenumber,
            DateOfBirth = userWithAvatar.User.DateOfBirth
        };
    }
}
