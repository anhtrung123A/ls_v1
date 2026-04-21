using app.Application.DTOs.Auth;
using app.Application.Errors;
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

    public async Task<CreateAuthUserResponseDto> CreateUserAsync(CreateAuthUserRequestDto request, CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var url = $"{baseUrl}{AuthConstants.CreateUserEndpoint}";
        var response = await _externalApiClient.PostAsync<CreateAuthUserRequestDto, CreateAuthUserResponseDto>(url, request, cancellationToken);

        if (response is null || response.Data is null)
        {
            throw new HttpRequestException(AppErrors.External.AuthServiceCallFailed);
        }

        return response;
    }
}
