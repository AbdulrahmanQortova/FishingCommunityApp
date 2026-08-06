using FishingHub.Mobile.Models.Api;
using FishingHub.Mobile.Models.Api.Auth;

namespace FishingHub.Mobile.Services.Interfaces;

public interface IAuthApiService
{
    Task<ApiResult<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}