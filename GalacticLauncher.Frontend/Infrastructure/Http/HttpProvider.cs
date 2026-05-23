using GalacticLauncher.Core;
using System;
using System.Net.Http;
using System.Threading;

namespace GalacticLauncher.Frontend.Infrastructure.Http;

internal static class HttpProvider
{
    public static HttpClient ApiClient { get; } = CreateApiClient();
    public static HttpClient DownloadClient { get; } = CreateDownloadClient();

    private static HttpClient CreateApiClient()
    {
        return new HttpClient(new HttpCustomHandler(
            denyInsecure: true,
            allowAutoRedirect: false,
            pinCertSHA256: Utils.CertThumbprint))
        {
            Timeout = TimeSpan.FromSeconds(15.0)
        };
    }

    private static HttpClient CreateDownloadClient()
    {
        return new HttpClient(new HttpCustomHandler(
            denyInsecure: true,
            allowAutoRedirect: true
            ))
        {
            Timeout = Timeout.InfiniteTimeSpan
        };
    }
}
