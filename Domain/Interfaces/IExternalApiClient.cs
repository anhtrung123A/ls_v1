namespace app.Domain.Interfaces;

public interface IExternalApiClient
{
    Task<TResponse?> GetAsync<TResponse>(string url, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest payload, CancellationToken cancellationToken = default);
}
