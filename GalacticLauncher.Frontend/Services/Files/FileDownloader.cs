using GalacticLauncher.Frontend.Domain.Exceptions;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Services.Files;

public interface IFileDownloader
{
    Task DownloadFileAsync(
        string url,
        string targetPath,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);
}

internal class FileDownloader(HttpClient httpClient) : IFileDownloader
{
    private const int BUFFER_SIZE = 8192;

    public async Task DownloadFileAsync(
        string url,
        string targetPath,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;

            using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using FileStream fileStream = new(
                targetPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                BUFFER_SIZE,
                FileOptions.Asynchronous);

            byte[] buffer = new byte[BUFFER_SIZE];
            long totalRead = 0;

            int bytesRead;
            while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                totalRead += bytesRead;

                if (totalBytes.HasValue)
                {
                    double current = totalRead;
                    double total = totalBytes.Value;

                    double percentage = current / total * 100.0;
                    progress?.Report(percentage);
                }
            }
        }
        catch (Exception ex)
        {
            int statusCode = (int)((ex as HttpRequestException)?.StatusCode ?? 0);

            throw new DownloadException($"Failed to download file: {url}", statusCode, ex);
        }
    }
}
