using System.Security.Claims;
using AutoMapper;
using app.Application.DTOs;
using app.Application.DTOs.Auth;
using app.Application.Errors;
using app.Domain.Constants;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class CreateBranchUserUseCase
{
    private readonly IBranchUserRepository _branchUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthUserService _authUserService;
    private readonly IEmailService _emailService;
    private readonly IFileUrlResolver _fileUrlResolver;
    private readonly IMapper _mapper;

    public CreateBranchUserUseCase(
        IBranchUserRepository branchUserRepository,
        IUserRepository userRepository,
        IAuthUserService authUserService,
        IEmailService emailService,
        IFileUrlResolver fileUrlResolver,
        IMapper mapper)
    {
        _branchUserRepository = branchUserRepository;
        _userRepository = userRepository;
        _authUserService = authUserService;
        _emailService = emailService;
        _fileUrlResolver = fileUrlResolver;
        _mapper = mapper;
    }

    public async Task<BranchUserDto> ExecuteAsync(
        ClaimsPrincipal user,
        CreateBranchUserDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Lastname))
        {
            throw new ArgumentException(AppErrors.User.LastnameRequired);
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ArgumentException(AppErrors.User.EmailRequired);
        }

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email.Trim(), cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException(AppErrors.User.EmailAlreadyExists);
        }

        var actorUserId = UserClaimResolver.TryGetUserId(user);
        var userEntity = new User
        {
            Firstname = dto.Firstname?.Trim(),
            Lastname = dto.Lastname.Trim(),
            Email = dto.Email.Trim(),
            Phonenumber = dto.Phonenumber?.Trim(),
            DateOfBirth = dto.DateOfBirth
        };

        var authResponse = await _authUserService.CreateUserAsync(new CreateAuthUserRequestDto
        {
            LoginId = userEntity.Email,
            RegisteredAtUtc = DateTime.UtcNow,
            RegisteredByEmail = userEntity.Email,
            UpdatedByEmail = userEntity.Email
        }, cancellationToken);

        var generatedPassword = authResponse.Data?.GeneratedPassword;
        if (string.IsNullOrWhiteSpace(generatedPassword))
        {
            throw new InvalidOperationException(AppErrors.External.AuthGeneratedPasswordMissing);
        }

        await _emailService.SendUserCreatedPasswordEmailAsync(
            userEntity.Email,
            CombineFullName(userEntity.Firstname, userEntity.Lastname),
            generatedPassword,
            cancellationToken);

        await _userRepository.AddAsync(userEntity, cancellationToken);
        await _userRepository.AssignRoleAsync(userEntity.Id, dto.RoleId, actorUserId, cancellationToken);

        var existed = await _branchUserRepository.ExistsAsync(userEntity.Id, dto.BranchId, cancellationToken);
        if (existed)
        {
            throw new InvalidOperationException(AppErrors.BranchUser.AlreadyExists);
        }

        var now = DateTime.UtcNow;
        var branchUser = new BranchUser
        {
            UserId = userEntity.Id,
            BranchId = dto.BranchId,
            Status = BranchUserStatusConstants.Active,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = actorUserId,
            UpdatedByUserId = actorUserId
        };

        await _branchUserRepository.AddAsync(branchUser, cancellationToken);
        return ToBranchUserUserDto(userEntity, branchUser, null);
    }

    private static string CombineFullName(string? firstName, string lastName)
    {
        return string.Join(" ", new[] { firstName?.Trim(), lastName.Trim() }
            .Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private BranchUserDto ToBranchUserUserDto(User user, BranchUser branchUser, string? avatarObjectKey)
    {
        var dto = _mapper.Map<BranchUserDto>(user);
        dto.BranchId = branchUser.BranchId;
        dto.Status = branchUser.Status;
        dto.AvatarUrl = _fileUrlResolver.BuildPublicUrl(avatarObjectKey);
        return dto;
    }
}
