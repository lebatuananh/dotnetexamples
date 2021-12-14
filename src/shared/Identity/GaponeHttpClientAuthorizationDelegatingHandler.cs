using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Shared.Identity;

public class GaponeHttpClientAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly TokenRetriever _tokenRetriever;

    public GaponeHttpClientAuthorizationDelegatingHandler(TokenRetriever tokenRetriever)
    {
        _tokenRetriever = tokenRetriever;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await SetToken(request, cancellationToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task SetToken(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var clientToken = await _tokenRetriever.RequestToken();
        if (!string.IsNullOrEmpty(clientToken))
            request.SetBearerToken(clientToken);
    }
}