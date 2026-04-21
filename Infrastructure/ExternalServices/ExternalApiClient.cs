using System.Net.Http.Json;
using app.Domain.Interfaces;

namespace app.Infrastructure.ExternalServices;

public class ExternalApiClient : IExternalApiClient
{
    private readonly HttpClient _httpClient;

    public ExternalApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TResponse?> GetAsync<TResponse>(string url, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessStatusCode(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest payload, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);
        await EnsureSuccessStatusCode(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    }

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = $"External API call failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {responseBody}";
        throw new HttpRequestException(message, null, response.StatusCode);
    }
}
