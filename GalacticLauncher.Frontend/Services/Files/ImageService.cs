using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Tools.Files;

namespace GalacticLauncher.Frontend.Services.Files
{
    public interface IImageService
    {
        Task<string?> GetImageAsync(long gameId, string url);
    }

    internal class ImageService(IFileDownloader downloader) : IImageService
    {
        private static readonly string ImagesFolder = Path.GetFullPath(Path.Combine(Utils.RootPath, "images"));
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".webp"];

        public async Task<string?> GetImageAsync(long gameId, string url)
        {
            string extension = Path.GetExtension(url).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            {
                return null;
            }

            string fileName = $"icon_{gameId}{extension}";
            string fullPath = Path.GetFullPath(Path.Combine(ImagesFolder, fileName));

            if (!Directory.Exists(ImagesFolder))
            {
                Directory.CreateDirectory(ImagesFolder);
            }
            else
            {
                var oldFiles = Directory.GetFiles(ImagesFolder, $"icon_{gameId}.*");
                foreach (var oldFile in oldFiles)
                {
                    File.Delete(oldFile);
                }
            }

            await downloader.DownloadFileAsync(url, fullPath);

            return fullPath;
        }
    }
}
