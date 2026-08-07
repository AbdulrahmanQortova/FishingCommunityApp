using System.Net.Http.Headers;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ISecureTokenStorage _tokenStorage;

    public AuthHeaderHandler(ISecureTokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _tokenStorage.GetAccessTokenAsync();

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}