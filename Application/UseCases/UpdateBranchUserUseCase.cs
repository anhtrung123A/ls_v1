using System.Security.Claims;
using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Constants;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class UpdateBranchUserUseCase
{
    private readonly IBranchUserRepository _branchUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileUrlResolver _fileUrlResolver;
    private readonly IMapper _mapper;

    public UpdateBranchUserUseCase(
        IBranchUserRepository branchUserRepository,
        IUserRepository userRepository,
        IFileUrlResolver fileUrlResolver,
        IMapper mapper)
    {
        _branchUserRepository = branchUserRepository;
        _userRepository = userRepository;
        _fileUrlResolver = fileUrlResolver;
        _mapper = mapper;
    }

    public async Task<BranchUserDto> ExecuteAsync(
        ClaimsPrincipal user,
        ulong id,
        UpdateBranchUserDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Status is not BranchUserStatusConstants.Active and not BranchUserStatusConstants.Blocked)
        {
            throw new ArgumentException(AppErrors.BranchUser.InvalidStatus);
        }

        var existing = await _branchUserRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new KeyNotFoundException(AppErrors.BranchUser.NotFound);
        }

        existing.Status = dto.Status;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedByUserId = UserClaimResolver.TryGetUserId(user);

        var updated = await _branchUserRepository.UpdateAsync(existing, cancellationToken);
        if (updated is null)
        {
            throw new KeyNotFoundException(AppErrors.BranchUser.NotFound);
        }

        var userEntity = await _userRepository.GetByIdAsync(updated.UserId, cancellationToken);
        if (userEntity is null)
        {
            throw new KeyNotFoundException(AppErrors.User.UserNotFoundByEmail);
        }

        var result = _mapper.Map<BranchUserDto>(userEntity);
        result.BranchId = updated.BranchId;
        result.Status = updated.Status;

        if (userEntity.AvatarFileId.HasValue)
        {
            var objectKeys = await _userRepository.GetFileObjectKeysByIdsAsync([userEntity.AvatarFileId.Value], cancellationToken);
            objectKeys.TryGetValue(userEntity.AvatarFileId.Value, out var avatarObjectKey);
            result.AvatarUrl = _fileUrlResolver.BuildPublicUrl(avatarObjectKey);
        }

        return result;
    }
}
