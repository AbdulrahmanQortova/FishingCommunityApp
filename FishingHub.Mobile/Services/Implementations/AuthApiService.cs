using FishingHub.Mobile.Models.Api;
using FishingHub.Mobile.Models.Api.Auth;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Services.Implementations;

public class AuthApiService : IAuthApiService
{
    private readonly IApiClient _apiClient;

    public AuthApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        return _apiClient.PostAsync<RegisterRequest, RegisterResponse>("api/v1/auth/register", request, cancellationToken);
    }

    public Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        return _apiClient.PostAsync<LoginRequest, LoginResponse>("api/v1/auth/login", request, cancellationToken);
    }
}