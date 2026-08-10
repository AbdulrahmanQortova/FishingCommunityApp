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

            var rawContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(rawContent))
            {
                return new ApiResult<TResponse>
                {
                    Succeeded = response.IsSuccessStatusCode,
                    Errors = response.IsSuccessStatusCode ? Array.Empty<string>() : new[] { $"Empty response (status {(int)response.StatusCode})." }
                };
            }

            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResult<TResponse>>(rawContent, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? new ApiResult<TResponse>
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