using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Services.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Services.Executables;

public interface IExecManager
{
    bool Exists(ExecInfo execInfo);
    void Delete(ExecInfo execInfo);
    Task Download(ExecInfo execInfo, IProgress<double> progress, CancellationToken cancellationToken = default);
    bool IsDownloading(ExecInfo execInfo);
    void Play(ExecInfo execInfo);
}

internal class ExecManager(
    IExecPathSystem execPathSystem,
    IFileDownloader fileDownloader,
    IFileHasher fileHasher,
    IFileDecompressor fileDecompressor) : IExecManager
{
    private const string TEMP_ZIP_FILE = "download_temp.zip";
    private const string READY_MARKER_FILE = "ready_marker.txt";

    private readonly HashSet<string> _downloadings = [];

    public bool Exists(ExecInfo execInfo)
    {
        string execPath = execPathSystem.PrepareExecPath(execInfo, false);

        string markerFilePath = Path.Combine(execPath, READY_MARKER_FILE);

        if (!IsDownloading(execInfo))
        {
            bool existsDir = Path.Exists(execPath);
            bool existsMarker = File.Exists(markerFilePath);

            if (existsDir && !existsMarker)
            {
                try { DeleteBody(execInfo); } // cleanup if marker is missing
                catch { }
            }

            return existsMarker;
        }

        return false;
    }

    public void Delete(ExecInfo execInfo)
    {
        if (Exists(execInfo))
            DeleteBody(execInfo);
    }

    public void DeleteBody(ExecInfo execInfo)
    {
        string gamePath = execPathSystem.PrepareGamePath(execInfo, false);
        string execPath = execPathSystem.PrepareExecPath(execInfo, false);

        if (Directory.Exists(execPath))
            Directory.Delete(execPath, true);

        if (Directory.Exists(gamePath) && !Directory.EnumerateDirectories(gamePath).Any())
            Directory.Delete(gamePath, true);
    }

    public async Task Download(ExecInfo execInfo, IProgress<double> progress, CancellationToken cancellationToken = default)
    {
        if (IsDownloading(execInfo))
            throw new InvalidOperationException("This exec is already being downloaded.");

        if (Exists(execInfo))
            return; // assume it has completed successfully

        string identity = execInfo.GetIdentity();

        _downloadings.Add(identity);

        string execPath = execPathSystem.PrepareExecPath(execInfo, true);
        string instancePath = execPathSystem.PrepareInstancePath(execInfo, true);

        string markerFilePath = Path.Combine(execPath, READY_MARKER_FILE);
        string zipFilePath = Path.Combine(execPath, TEMP_ZIP_FILE);

        try
        {
            await fileDownloader.DownloadFileAsync(
                execInfo.DownloadUrl,
                zipFilePath,
                progress,
                cancellationToken);

            if (execInfo.Sha256Hash != null) // verify only when hash is provided
            {
                string fileHash = await fileHasher.HashSha256Async(
                    zipFilePath,
                    cancellationToken);

                if (!string.Equals(fileHash, execInfo.Sha256Hash,
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new SecurityException("Executable hash mismatch has occurred.");
                }
            }

            await fileDecompressor.UnpackZipAsync(
                zipFilePath,
                instancePath,
                cancellationToken);

            string markerContents = DateTime.Now.ToString();
            File.WriteAllText(markerFilePath, markerContents); // mark as ready
        }
        catch (Exception ex)
        {
            try { Directory.Delete(execPath, true); } catch { } // cleanup

            throw ex is DownloadException
                ? ex
                : new DownloadException("Downloading executable failed.", ex);
        }
        finally
        {
            try { File.Delete(zipFilePath); } catch { }
            _downloadings.Remove(identity);
        }
    }

    public bool IsDownloading(ExecInfo execInfo)
    {
        string identity = execInfo.GetIdentity();

        return _downloadings.Contains(identity);
    }

    public void Play(ExecInfo execInfo)
    {
        throw new NotImplementedException();
    }
}
