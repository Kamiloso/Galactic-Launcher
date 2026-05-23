using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Infrastructure.Files;

public interface IFileHasher
{
    Task<string> HashSha256Async(string filePath, CancellationToken cancellationToken);
}

internal class FileHasher : IFileHasher
{
    public async Task<string> HashSha256Async(string filePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        using var stream = File.OpenRead(filePath);

        byte[] bytes = await SHA256.HashDataAsync(stream, cancellationToken);

        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
