using System.Net.Http.Json;
using FishingHub.Mobile.Models.Api;
using FishingHub.Mobile.Models.Api.Auth;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Services.Implementations;

public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/auth/register", request, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<ApiResult<RegisterResponse>>(cancellationToken: cancellationToken);

            return result ?? new ApiResult<RegisterResponse> { Succeeded = false, Errors = new[] { "Unexpected server response." } };
        }
        catch (Exception ex)
        {
            // Network failure, timeout, unreachable server — surface as a normal
            // failed result rather than an unhandled exception crashing the app.
            return new ApiResult<RegisterResponse>
            {
                Succeeded = false,
                Errors = new[] { $"Network error: {ex.Message}" }
            };
        }
    }
}