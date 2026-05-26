using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Tools.Files;

namespace GalacticLauncher.Frontend.Services.Files
{
    public interface IImageService
    {
        Task<string> DownloadImageAsync(long gameId, string url);
    }

    internal class ImageService(IFileDownloader downloader) : IImageService
    {
        public async Task<string> DownloadImageAsync(long gameId, string url)
        {
            if (IsAllowedExtension(url, out string extension))
            {
                throw new DownloadException("Unsupported image format.");
            }

            string imagesPath = Path.Combine(Utils.RootPath, "images");
            string filePath = Path.Combine(imagesPath, $"icon_{gameId}{extension}");

            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }
            else
            {
                string[] oldFiles = Directory.GetFiles(imagesPath, $"icon_{gameId}.*");
                foreach (var oldFile in oldFiles)
                {
                    File.Delete(oldFile);
                }
            }

            await downloader.DownloadFileAsync(url, filePath);

            return filePath;
        }

        private static bool IsAllowedExtension(string url, out string extension)
        {
            string[] allowed = [".jpg", ".jpeg", ".png", ".bmp", ".webp"];

            extension = $"{Path.GetExtension(url)}".ToLowerInvariant();
            return allowed.Contains(extension);
        }
    }
}
