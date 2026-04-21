using app.Application.DTOs;
using app.Application.DTOs.Auth;
using app.Application.Errors;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthUserService _authUserService;
    private readonly IEmailService _emailService;

    public CreateUserUseCase(
        IUserRepository userRepository,
        IAuthUserService authUserService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _authUserService = authUserService;
        _emailService = emailService;
    }

    public async Task<UserDto> ExecuteAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Fullname))
        {
            throw new ArgumentException(AppErrors.User.FullnameRequired);
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

        var user = new User
        {
            Fullname = dto.Fullname.Trim(),
            Firstname = dto.Firstname?.Trim(),
            Email = dto.Email.Trim(),
            Phonenumber = dto.Phonenumber?.Trim(),
            DateOfBirth = dto.DateOfBirth
        };

        var authResponse = await _authUserService.CreateUserAsync(new CreateAuthUserRequestDto
        {
            LoginId = user.Email,
            RegisteredAtUtc = DateTime.UtcNow,
            RegisteredByEmail = user.Email,
            UpdatedByEmail = user.Email
        }, cancellationToken);

        var generatedPassword = authResponse.Data?.GeneratedPassword;
        if (string.IsNullOrWhiteSpace(generatedPassword))
        {
            throw new InvalidOperationException(AppErrors.External.AuthGeneratedPasswordMissing);
        }

        await _emailService.SendUserCreatedPasswordEmailAsync(
            user.Email,
            user.Fullname,
            generatedPassword,
            cancellationToken);

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Fullname = user.Fullname,
            Firstname = user.Firstname,
            Email = user.Email,
            Phonenumber = user.Phonenumber,
            DateOfBirth = user.DateOfBirth,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
