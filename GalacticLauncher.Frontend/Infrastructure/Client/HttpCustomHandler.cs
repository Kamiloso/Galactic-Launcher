using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Infrastructure.Client;

internal class HttpCustomHandler(bool denyInsecure, bool allowAutoRedirect, string? pinCertSHA256 = null) : DelegatingHandler(
    new HttpClientHandler()
    {
        AllowAutoRedirect = allowAutoRedirect,
        ServerCertificateCustomValidationCallback = pinCertSHA256 == null ? default : (request, cert, chain, errors) =>
        {
            string requiredHash = pinCertSHA256;
            string certHash = cert?.GetCertHashString(HashAlgorithmName.SHA256) ?? "";

            return string.Equals(requiredHash, certHash, StringComparison.OrdinalIgnoreCase);
        },
    })
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (denyInsecure)
        {
            if (request.RequestUri?.Scheme != Uri.UriSchemeHttps)
                throw new InvalidOperationException("Insecure request denied.");
        }

        return base.SendAsync(request, cancellationToken);
    }
}
