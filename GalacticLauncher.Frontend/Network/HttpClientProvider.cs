using GalacticLauncher.Core;
using System;
using System.Net.Http;
using System.Security.Cryptography;

namespace GalacticLauncher.Frontend.Network
{
    internal static class HttpClientProvider
    {
        private static double HttpTimeout => 5.0;

        private static readonly Lazy<HttpClient> _httpClient = new(() =>
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
        });

        public static HttpClient HttpClient => _httpClient.Value;
    }
}
