using GalacticLauncher.Core;
using System;
using System.Net.Http;
using System.Security.Cryptography;

namespace GalacticLauncher.Frontend.Network;

internal static class HttpClientProvider
{
    public static double HttpTimeout => 10.0;
    public static HttpClient HttpClient { get; } = CreateHttpClient();

    private static HttpClient CreateHttpClient()
    {
        HttpClientHandler handler = new()
        {
            ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
            {
                bool isHttps = request.RequestUri?.Scheme == Uri.UriSchemeHttps;
                bool certOk = cert?.GetCertHashString(HashAlgorithmName.SHA256).Equals(
                    Utils.CertThumbprint, StringComparison.OrdinalIgnoreCase) == true;

                return isHttps && certOk;
            }
        };

        HttpClient client = new(handler)
        {
            Timeout = TimeSpan.FromSeconds(HttpTimeout)
        };

        return client;
    }
}
