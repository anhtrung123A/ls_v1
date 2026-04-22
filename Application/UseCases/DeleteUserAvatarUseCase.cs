using System.Security.Claims;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class DeleteUserAvatarUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;

    public DeleteUserAvatarUseCase(
        IUserRepository userRepository,
        IFileStorageService fileStorageService)
    {
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<DeleteUserAvatarResultDto> ExecuteAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var email = UserClaimResolver.GetEmailOrThrow(user);
        var userWithAvatar = await _userRepository.GetWithAvatarByEmailAsync(email, cancellationToken);

        if (userWithAvatar.User is null)
        {
            throw new KeyNotFoundException(AppErrors.User.UserNotFoundByEmail);
        }

        var previousAvatar = await _userRepository.RemoveAvatarAsync(userWithAvatar.User.Id, cancellationToken);
        if (!string.IsNullOrWhiteSpace(previousAvatar?.ObjectKey))
        {
            await _fileStorageService.DeleteAsync(previousAvatar.ObjectKey, cancellationToken);
        }

        return new DeleteUserAvatarResultDto
        {
            Deleted = previousAvatar is not null
        };
    }
}
