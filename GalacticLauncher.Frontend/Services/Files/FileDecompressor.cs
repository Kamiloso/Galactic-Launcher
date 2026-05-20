using GalacticLauncher.Core;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Services.Files;

public interface IFileDecompressor
{
    Task UnpackZipAsync(string zipFilePath, string targetDirectory, CancellationToken cancellationToken);
}

internal class FileDecompressor : IFileDecompressor
{
    public async Task UnpackZipAsync(string zipFilePath, string targetDirectory, CancellationToken cancellationToken)
    {
        if (!File.Exists(zipFilePath))
            throw new FileNotFoundException("File not found.", zipFilePath);

        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        using var fileStream = File.OpenRead(zipFilePath);
        using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);

        foreach (var entry in archive.Entries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (((entry.ExternalAttributes >> 16) & 0xF000) == 0xA000)
                throw new SecurityException($"Symlink detected in a ZIP file: {entry.FullName}");

            string destination = Path.GetFullPath(Path.Combine(targetDirectory, entry.FullName));

            if (!Utils.IsPathInside(destination, targetDirectory))
                throw new SecurityException($"Entry is trying to extract outside of the target directory: {entry.FullName}");

            if (string.IsNullOrEmpty(entry.Name))
            {
                Directory.CreateDirectory(destination);
                continue;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

            using var entryStream = entry.Open();
            using var destinationStream = File.Create(destination);

            await entryStream.CopyToAsync(destinationStream, cancellationToken);
        }
    }
}
