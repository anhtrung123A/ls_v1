using app.Application.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> ExecuteAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Fullname))
        {
            throw new ArgumentException("Fullname is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email.Trim(), cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("Email already exists.");
        }

        var user = new User
        {
            Fullname = dto.Fullname.Trim(),
            Firstname = dto.Firstname?.Trim(),
            Email = dto.Email.Trim(),
            Phonenumber = dto.Phonenumber?.Trim(),
            DateOfBirth = dto.DateOfBirth
        };

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
