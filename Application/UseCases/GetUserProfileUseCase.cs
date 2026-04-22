using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetUserProfileUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserProfileUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto> ExecuteAsync(
        System.Security.Claims.ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var email = UserClaimResolver.GetEmailOrThrow(user);

        var dbUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (dbUser is null)
        {
            throw new KeyNotFoundException(AppErrors.User.UserNotFoundByEmail);
        }

        return new UserProfileDto
        {
            Id = dbUser.Id,
            Firstname = dbUser.Firstname,
            Lastname = dbUser.Lastname,
            Email = dbUser.Email,
            Phonenumber = dbUser.Phonenumber,
            DateOfBirth = dbUser.DateOfBirth
        };
    }
}
