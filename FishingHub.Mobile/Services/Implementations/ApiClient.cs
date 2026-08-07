using System.Net.Http.Json;
using FishingHub.Mobile.Models.Api;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Services.Implementations;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<TResponse>> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync<TResponse>(() => _httpClient.GetAsync(endpoint, cancellationToken));
    }

    public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync<TResponse>(() => _httpClient.PostAsJsonAsync(endpoint, body, cancellationToken));
    }

    public async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync<TResponse>(() => _httpClient.PutAsJsonAsync(endpoint, body, cancellationToken));
    }

    public async Task<ApiResult<object>> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync<object>(() => _httpClient.DeleteAsync(endpoint, cancellationToken));
    }

    private async Task<ApiResult<TResponse>> ExecuteAsync<TResponse>(Func<Task<HttpResponseMessage>> sendRequest)
    {
        try
        {
            using var response = await sendRequest();

            // Some successful responses (e.g. 204 No Content, or a bare 200 with an
            // empty body) won't deserialize to ApiResult<T> — treat those as a
            // generic success rather than a deserialization failure.
            if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength is 0 or null)
            {
                return new ApiResult<TResponse> { Succeeded = true };
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResult<TResponse>>();

            if (result is not null)
            {
                return result;
            }

            return new ApiResult<TResponse>
            {
                Succeeded = false,
                Errors = new[] { $"Unexpected response (status {(int)response.StatusCode})." }
            };
        }
        catch (Exception ex)
        {
            return new ApiResult<TResponse>
            {
                Succeeded = false,
                Errors = new[] { $"Network error: {ex.Message}" }
            };
        }
    }
}