using app.Application.DTOs.Auth;

namespace app.Domain.Interfaces;

public interface IAuthUserService
{
    Task CreateUserAsync(CreateAuthUserRequestDto request, CancellationToken cancellationToken = default);
}
