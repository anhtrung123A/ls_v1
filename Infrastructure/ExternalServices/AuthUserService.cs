using app.Application.DTOs.Auth;
using app.Domain.Constants;
using app.Domain.Interfaces;
using app.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace app.Infrastructure.ExternalServices;

public class AuthUserService : IAuthUserService
{
    private readonly IExternalApiClient _externalApiClient;
    private readonly AuthServiceOptions _options;

    public AuthUserService(IExternalApiClient externalApiClient, IOptions<AuthServiceOptions> options)
    {
        _externalApiClient = externalApiClient;
        _options = options.Value;
    }

    public async Task CreateUserAsync(CreateAuthUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var url = $"{baseUrl}{AuthConstants.CreateUserEndpoint}";
        await _externalApiClient.PostAsync<CreateAuthUserRequestDto, object>(url, request, cancellationToken);
    }
}
