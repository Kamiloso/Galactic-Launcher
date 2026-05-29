using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Tools.Files;

namespace GalacticLauncher.Frontend.Services.Images;

public interface IImageProvider
{
    Task<string> GetImagePathAsync(string url);
}

internal class ImageProvider(IFileDownloader fileDownloader) : IImageProvider
{
    private const string IMG_FOLDER = "Images";

    private readonly Dictionary<string, Task<string>> _downloads = [];
    private readonly Dictionary<string, string> _ramCache = [];

    public async Task<string> GetImagePathAsync(string url)
    {
        string imgPath = Path.Combine(Utils.RootPath, IMG_FOLDER);
        string uniqueFileName = UrlToUniqueFileName(url);

        if (!Directory.Exists(imgPath))
        {
            Directory.CreateDirectory(imgPath);
        }

        if (_downloads.Count == 0) // clean up .tmp files
        {
            Directory.EnumerateFiles(imgPath, "*.tmp")
                .ToList()
                .ForEach(File.Delete);
        }

        if (!_downloads.TryGetValue(uniqueFileName, out var download))
        {
            download = _ramCache.TryGetValue(uniqueFileName, out var cachedPath)
                ? Task.FromResult(cachedPath)
                : DownloadToPathAsync(url, imgPath, uniqueFileName);

            _downloads.Add(uniqueFileName, download);
        }

        try
        {
            string imgFilePath = await download;
            
            if (!File.Exists(imgFilePath))
            {
                throw new DownloadException($"Downloaded file does not exist: {imgFilePath}");
            }

            return imgFilePath;
        }
        finally
        {
            _downloads.Remove(uniqueFileName);
        }
    }

    private async Task<string> DownloadToPathAsync(string url, string imgPath, string uniqueFileName)
    {
        string tmpFilePath = Path.Combine(imgPath, uniqueFileName + ".tmp");
        string imgFilePath = Path.Combine(imgPath, uniqueFileName);

        try
        {
            await fileDownloader.DownloadFileAsync(url, tmpFilePath);

            File.Move(tmpFilePath, imgFilePath, true);
        }
        catch (DownloadException)
        {
            try { File.Delete(tmpFilePath); } catch { }

            if (!File.Exists(imgFilePath)) throw;

            // ELSE: The file was already
            // downloaded and it's stored on disk.
        }

        _ramCache[uniqueFileName] = imgFilePath;

        return imgFilePath;
    }

    private static string UrlToUniqueFileName(string url)
    {
        byte[] data = Encoding.UTF8.GetBytes(url);
        byte[] hash = SHA256.HashData(data);

        string strHash = Convert
            .ToHexString(hash)
            .ToLowerInvariant();

        return $"{strHash[..16]}.img";
    }
}
