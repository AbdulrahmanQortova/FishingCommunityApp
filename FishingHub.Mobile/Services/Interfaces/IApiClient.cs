using FishingHub.Mobile.Models.Api;

namespace FishingHub.Mobile.Services.Interfaces;

public interface IApiClient
{
    Task<ApiResult<TResponse>> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
    Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken cancellationToken = default);
    Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken cancellationToken = default);
    Task<ApiResult<object>> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}