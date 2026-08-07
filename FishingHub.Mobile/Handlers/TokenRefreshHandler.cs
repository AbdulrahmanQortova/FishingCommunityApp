using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FishingHub.Mobile.Models.Api;
using FishingHub.Mobile.Models.Api.Auth;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Handlers;

public class TokenRefreshHandler : DelegatingHandler
{
    private readonly ISecureTokenStorage _tokenStorage;
    private readonly IServiceProvider _serviceProvider;

    // Ensures only one refresh attempt happens at a time, even if multiple requests
    // fail with 401 simultaneously (e.g. a page fires several parallel API calls).
    // Without this, we'd race to refresh the same token multiple times, and the
    // backend's refresh-token rotation would invalidate all but the first attempt.
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    public TokenRefreshHandler(ISecureTokenStorage tokenStorage, IServiceProvider serviceProvider)
    {
        _tokenStorage = tokenStorage;
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Clone the request body up front, since HttpRequestMessage can't be resent
        // once its content stream has already been consumed by the first attempt.
        var originalRequestClone = await CloneRequestAsync(request);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        await RefreshLock.WaitAsync(cancellationToken);

        try
        {
            // Another request might have already refreshed the token while we were
            // waiting for the lock — check if the stored access token changed since
            // we started, and if so, just retry with it instead of refreshing again.
            var refreshed = await TryRefreshTokenAsync(cancellationToken);

            if (!refreshed)
            {
                // Refresh itself failed (refresh token expired/revoked too) —
                // the user needs to log in again. Clear the session and surface
                // the original 401 as-is; the UI layer decides how to react.
                var currentUserService = _serviceProvider.GetRequiredService<ICurrentUserService>();
                await currentUserService.ClearAsync();

                return response;
            }
        }
        finally
        {
            RefreshLock.Release();
        }

        // Retry the original request once, now with the freshly refreshed token.
        var retryRequest = originalRequestClone;
        var newAccessToken = await _tokenStorage.GetAccessTokenAsync();

        if (!string.IsNullOrEmpty(newAccessToken))
        {
            retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
        }

        return await base.SendAsync(retryRequest, cancellationToken);
    }

    private async Task<bool> TryRefreshTokenAsync(CancellationToken cancellationToken)
    {
        var expiredAccessToken = await _tokenStorage.GetAccessTokenAsync();
        var refreshToken = await _tokenStorage.GetRefreshTokenAsync();

        if (string.IsNullOrEmpty(expiredAccessToken) || string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }

        try
        {
            // A plain, unauthenticated HttpClient — deliberately NOT routed through
            // this same handler chain, to avoid infinite recursion if the refresh
            // call itself somehow returned a 401.
            using var plainClient = new HttpClient { BaseAddress = InnerHandlerBaseAddress };

            var refreshRequest = new RefreshTokenRequest
            {
                AccessToken = expiredAccessToken,
                RefreshToken = refreshToken
            };

            var httpResponse = await plainClient.PostAsJsonAsync("api/v1/auth/refresh-token", refreshRequest, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return false;
            }

            var result = await httpResponse.Content.ReadFromJsonAsync<ApiResult<RefreshTokenResponse>>(cancellationToken: cancellationToken);

            if (result is null || !result.Succeeded || result.Data is null)
            {
                return false;
            }

            await _tokenStorage.SaveTokensAsync(result.Data.AccessToken, result.Data.RefreshToken, result.Data.AccessTokenExpiresOn);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);

        if (original.Content is not null)
        {
            var contentBytes = await original.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(contentBytes);

            foreach (var header in original.Content.Headers)
            {
                clone.Content.Headers.Add(header.Key, header.Value);
            }
        }

        foreach (var header in original.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }

    // Exposed as a static field set once at startup, since this handler doesn't have
    // direct access to the DI-configured base address of the "real" HttpClient here.
    public static Uri? InnerHandlerBaseAddress { get; set; }
}