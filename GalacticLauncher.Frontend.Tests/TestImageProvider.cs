using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.Tools.Files;
using Moq;

namespace GalacticLauncher.Frontend.Tests.Services
{
    public class ImageProviderTests : IDisposable
    {
        private readonly Mock<IFileDownloader> _fileDownloaderMock = new();
        private readonly string _imagesDirectoryPath;

        public ImageProviderTests()
        {
            _imagesDirectoryPath = Path.Combine(Utils.RootPath, "Images");

            if (Directory.Exists(_imagesDirectoryPath))
            {
                Directory.Delete(_imagesDirectoryPath, true);
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(_imagesDirectoryPath))
            {
                try { Directory.Delete(_imagesDirectoryPath, true); } catch { }
            }
        }

        private ImageProvider CreateProvider()
        {
            return new ImageProvider(_fileDownloaderMock.Object);
        }

        /// <summary>
        ///  DOWNLOAD AND CACHE TESTS
        /// </summary>

        [Fact]
        public async Task GetImagePathAsync_ShouldDownloadFile_AndReturnPath_WhenFirstTime()
        {
            var provider = CreateProvider();
            string imageUrl = "https://galactic-launcher.com/assets/banner.png";

            _fileDownloaderMock
                .Setup(d => d.DownloadFileAsync(imageUrl, It.IsAny<string>(), It.IsAny<IProgress<DownloadProgressData>>(), It.IsAny<CancellationToken>()))
                .Callback<string, string, IProgress<DownloadProgressData>, CancellationToken>((url, tmpPath, prog, token) =>
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(tmpPath)!);
                    File.WriteAllText(tmpPath, "fake image payload");
                })
                .Returns(Task.CompletedTask);

            string resultPath = await provider.GetImagePathAsync(imageUrl);

            Assert.True(File.Exists(resultPath), "File should be on the disk.");
            Assert.StartsWith(_imagesDirectoryPath, resultPath);
            Assert.EndsWith(".img", resultPath);

            _fileDownloaderMock.Verify(d => d.DownloadFileAsync(imageUrl, It.IsAny<string>(), It.IsAny<IProgress<DownloadProgressData>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetImagePathAsync_ShouldUseRamCache_AndNotDownloadTwice_WhenCalledSequentially()
        {
            var provider = CreateProvider();
            string imageUrl = "https://galactic-launcher.com/assets/logo.png";

            _fileDownloaderMock
                .Setup(d => d.DownloadFileAsync(imageUrl, It.IsAny<string>(), It.IsAny<IProgress<DownloadProgressData>>(), It.IsAny<CancellationToken>()))
                .Callback<string, string, IProgress<DownloadProgressData>, CancellationToken>((url, tmpPath, prog, token) =>
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(tmpPath)!);
                    File.WriteAllText(tmpPath, "fake image payload");
                })
                .Returns(Task.CompletedTask);

            string firstPath = await provider.GetImagePathAsync(imageUrl);

            string secondPath = await provider.GetImagePathAsync(imageUrl);

            Assert.Equal(firstPath, secondPath);
            _fileDownloaderMock.Verify(d => d.DownloadFileAsync(imageUrl, It.IsAny<string>(), It.IsAny<IProgress<DownloadProgressData>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetImagePathAsync_ShouldDeduplicateConcurrentRequests_WhenCalledSimultaneously()
        {
            var provider = CreateProvider();
            string imageUrl = "https://galactic-launcher.com/assets/background.png";

            var tcs = new TaskCompletionSource();

            _fileDownloaderMock
                .Setup(d => d.DownloadFileAsync(imageUrl, It.IsAny<string>(), It.IsAny<IProgress<DownloadProgressData>>(), It.IsAny<CancellationToken>()))
                .Callback<string, string, IProgress<DownloadProgressData>, CancellationToken>((url, tmpPath, prog, token) =>
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(tmpPath)!);
                    File.WriteAllText(tmpPath, "concurrent image payload");
                })
                .Returns(tcs.Task);

            Task<string> task1 = provider.GetImagePathAsync(imageUrl);
            Task<string> task2 = provider.GetImagePathAsync(imageUrl);

            tcs.SetResult();

            string[] paths = await Task.WhenAll(task1, task2);

            Assert.Equal(paths[0], paths[1]);
            _fileDownloaderMock.Verify(d => d.DownloadFileAsync(imageUrl, It.IsAny<string>(), It.IsAny<IProgress<DownloadProgressData>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        ///  FILE CLEANING TESTS
        /// </summary>

        [Fact]
        public async Task GetImagePathAsync_ShouldCleanOrphanedTmpFiles_WhenDownloadsCountIsZero()
        {
            var provider = CreateProvider();
            string imageUrl = "https://galactic-launcher.com/assets/avatar.png";

            Directory.CreateDirectory(_imagesDirectoryPath);
            string orphanedTmpFile = Path.Combine(_imagesDirectoryPath, "orphaned_old_file.tmp");
            File.WriteAllText(orphanedTmpFile, "old junk");

            _fileDownloaderMock
                .Setup(d => d.DownloadFileAsync(imageUrl, It.IsAny<string>(), It.IsAny<IProgress<DownloadProgressData>>(), It.IsAny<CancellationToken>()))
                .Callback<string, string, IProgress<DownloadProgressData>, CancellationToken>((url, tmpPath, prog, token) =>
                {
                    File.WriteAllText(tmpPath, "new image payload");
                })
                .Returns(Task.CompletedTask);

            await provider.GetImagePathAsync(imageUrl);

            Assert.False(File.Exists(orphanedTmpFile), "The orphaned .tmp file should be cleaned up from the disk.");
        }

        [Fact]
        public async Task GetImagePathAsync_ShouldFallbackToExistingFile_WhenDownloadThrowsDownloadException_ButFileExistsOnDisk()
        {
            var provider = CreateProvider();
            string imageUrl = "https://galactic-launcher.com/assets/cached_fail.png";

            _fileDownloaderMock
                .Setup(d => d.DownloadFileAsync(imageUrl, It.IsAny<string>(), It.IsAny<IProgress<DownloadProgressData>>(), It.IsAny<CancellationToken>()))
                .Callback<string, string, IProgress<DownloadProgressData>, CancellationToken>((url, tmpPath, prog, token) =>
                {
                    string targetImgPath = tmpPath.Replace(".tmp", "");
                    Directory.CreateDirectory(Path.GetDirectoryName(targetImgPath)!);
                    File.WriteAllText(targetImgPath, "previously downloaded image data");
                })
                .Throws(new DownloadException("Network connection failed."));

            string resultPath = await provider.GetImagePathAsync(imageUrl);

            Assert.True(File.Exists(resultPath));
        }

        [Fact]
        public async Task GetImagePathAsync_ShouldThrowDownloadException_WhenFileDoesNotExistAfterDownload()
        {
            var provider = CreateProvider();
            string imageUrl = "https://galactic-launcher.com/assets/ghost.png";

            _fileDownloaderMock
                .Setup(d => d.DownloadFileAsync(imageUrl, It.IsAny<string>(), It.IsAny<IProgress<DownloadProgressData>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await Assert.ThrowsAsync<DownloadException>(() => provider.GetImagePathAsync(imageUrl));
        }
    }
}