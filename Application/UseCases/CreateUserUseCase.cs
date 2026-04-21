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

    public CreateUserUseCase(IUserRepository userRepository, IAuthUserService authUserService)
    {
        _userRepository = userRepository;
        _authUserService = authUserService;
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

        await _authUserService.CreateUserAsync(new CreateAuthUserRequestDto
        {
            LoginId = user.Email,
            RegisteredAtUtc = DateTime.UtcNow,
            RegisteredByEmail = user.Email,
            UpdatedByEmail = user.Email
        }, cancellationToken);

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
