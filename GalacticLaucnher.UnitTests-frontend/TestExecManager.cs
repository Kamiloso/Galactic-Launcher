using System.Diagnostics;
using System.Security;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.Tools.Files;
using Moq;

namespace GalacticLauncher.Frontend.Tests.Services.Executables
{
    public class ExecManagerTests : IDisposable
    {
        private readonly Mock<IExecPathSystem> _execPathSystemMock = new();
        private readonly Mock<IExecRunner> _execRunnerMock = new();
        private readonly Mock<IFileDownloader> _fileDownloaderMock = new();
        private readonly Mock<IFileHasher> _fileHasherMock = new();
        private readonly Mock<IFileDecompressor> _fileDecompressorMock = new();

        private readonly string _testExecutionPath;
        private const string READY_MARKER_FILE = "ready_marker.txt";

        public ExecManagerTests()
        {
            _testExecutionPath = Path.Combine(Path.GetTempPath(), $"ExecManagerTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testExecutionPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testExecutionPath))
            {
                try { Directory.Delete(_testExecutionPath, true); } catch { }
            }
        }

        private ExecManager CreateManager()
        {
            return new ExecManager(
                _execPathSystemMock.Object,
                _execRunnerMock.Object,
                _fileDownloaderMock.Object,
                _fileHasherMock.Object,
                _fileDecompressorMock.Object
            );
        }

        private ExecInfo CreateTestExecInfo(string? sha256Hash = null)
        {
            return new ExecInfo
            {
                GameId = 1,
                GameName = "Galactic Game",
                GameUnique = "galactic_game_unique",

                VersionId = 100,
                VersionName = "Version 1.0",
                VersionUnique = "v1_0_unique",
                CliArgs = "--skip-intro --fullscreen",
                DownloadUrl = "http://galactic-launcher.com/game.zip",
                ExecLocation = "bin/game.exe",
                Sha256Hash = sha256Hash
            };
        }

        [Fact]
        public void Exists_ShouldReturnTrue_WhenMarkerFileExists()
        {
            var manager = CreateManager();
            var execInfo = CreateTestExecInfo();

            _execPathSystemMock.Setup(e => e.PrepareExecPath(execInfo, false)).Returns(_testExecutionPath);

            File.WriteAllText(Path.Combine(_testExecutionPath, READY_MARKER_FILE), "2026-06-10 12:00:00");

            bool result = manager.Exists(execInfo);

            Assert.True(result);
        }

        [Fact]
        public void Exists_ShouldReturnFalseAndCleanup_WhenDirectoryExistsButMarkerIsMissing()
        {
            var manager = CreateManager();
            var execInfo = CreateTestExecInfo();

            string gamePath = Path.Combine(_testExecutionPath, "GameDir");
            string execPath = Path.Combine(gamePath, "ExecDir");
            Directory.CreateDirectory(execPath);

            _execPathSystemMock.Setup(e => e.PrepareExecPath(execInfo, false)).Returns(execPath);
            _execPathSystemMock.Setup(e => e.PrepareGamePath(execInfo, false)).Returns(gamePath);

            bool result = manager.Exists(execInfo);

            Assert.False(result);
            Assert.False(Directory.Exists(execPath), "Catalog without marker should be deleted by auto-cleanup.");
        }

        [Fact]
        public async Task DownloadAsync_ShouldRunFullPipeline_AndCreateMarkerFile_WhenSuccessful()
        {
            var manager = CreateManager();
            var execInfo = CreateTestExecInfo(sha256Hash: "VALID_HASH");
            var progressMock = new Mock<IProgress<DownloadProgressData>>();

            _execPathSystemMock.Setup(e => e.PrepareExecPath(execInfo, It.IsAny<bool>())).Returns(_testExecutionPath);
            _execPathSystemMock.Setup(e => e.PrepareInstancePath(execInfo, It.IsAny<bool>())).Returns(_testExecutionPath);

            _fileHasherMock.Setup(h => h.HashSha256Async(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync("VALID_HASH");

            _fileDecompressorMock
                .Setup(d => d.UnpackZipAsync(It.IsAny<string>(), _testExecutionPath, It.IsAny<CancellationToken>()))
                .Callback<string, string, CancellationToken>((zipPath, extractPath, token) =>
                {
                    Directory.CreateDirectory(extractPath);
                })
                .Returns(Task.CompletedTask);

            await manager.DownloadAsync(execInfo, progressMock.Object, CancellationToken.None);

            string expectedMarkerPath = Path.Combine(_testExecutionPath, READY_MARKER_FILE);
            Assert.True(File.Exists(expectedMarkerPath), "Marker should have been generated.");

            _fileDownloaderMock.Verify(d => d.DownloadFileAsync(execInfo.DownloadUrl, It.IsAny<string>(), progressMock.Object, It.IsAny<CancellationToken>()), Times.Once);
            _fileDecompressorMock.Verify(d => d.UnpackZipAsync(It.IsAny<string>(), _testExecutionPath, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DownloadAsync_ShouldThrowSecurityException_WhenHashMismatchOccurs()
        {
            var manager = CreateManager();
            var execInfo = CreateTestExecInfo(sha256Hash: "EXPECTED_GOOD_HASH");
            var progressMock = new Mock<IProgress<DownloadProgressData>>();

            _execPathSystemMock.Setup(e => e.PrepareExecPath(execInfo, It.IsAny<bool>())).Returns(_testExecutionPath);
            _execPathSystemMock.Setup(e => e.PrepareInstancePath(execInfo, It.IsAny<bool>())).Returns(_testExecutionPath);

            _fileHasherMock.Setup(h => h.HashSha256Async(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync("MALICIOUS_OR_CORRUPTED_HASH");

            var exception = await Assert.ThrowsAsync<DownloadException>(() =>
                manager.DownloadAsync(execInfo, progressMock.Object, CancellationToken.None)
            );

            Assert.IsType<SecurityException>(exception.InnerException);
        }

        [Fact]
        public async Task DownloadAsync_ShouldThrowInvalidOperationException_WhenAlreadyDownloading()
        {
            var manager = CreateManager();
            var execInfo = CreateTestExecInfo();
            var progressMock = new Mock<IProgress<DownloadProgressData>>();

            _execPathSystemMock.Setup(e => e.PrepareExecPath(execInfo, It.IsAny<bool>())).Returns(_testExecutionPath);
            _execPathSystemMock.Setup(e => e.PrepareInstancePath(execInfo, It.IsAny<bool>())).Returns(_testExecutionPath);

            var tcs = new TaskCompletionSource();
            _fileDownloaderMock.Setup(d => d.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IProgress<DownloadProgressData>>(), It.IsAny<CancellationToken>()))
                               .Returns(tcs.Task);

            var firstDownloadTask = manager.DownloadAsync(execInfo, progressMock.Object, CancellationToken.None);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                manager.DownloadAsync(execInfo, progressMock.Object, CancellationToken.None)
            );

            tcs.SetResult();
            try { await firstDownloadTask; } catch { }
        }

        [Fact]
        public void Play_ShouldReturnProcess_WhenExecutableIsFoundAndStarted()
        {
            var manager = CreateManager();
            var execInfo = CreateTestExecInfo();
            string expectedExecFilePath = Path.Combine(_testExecutionPath, "GameExecutable.exe");

            _execPathSystemMock.Setup(e => e.FindExecFilePath(execInfo)).Returns(expectedExecFilePath);

            var dummyProcess = new Process();
            _execRunnerMock.Setup(r => r.RunProcess(expectedExecFilePath, execInfo.CliArgs)).Returns(dummyProcess);

            var resultProcess = manager.Play(execInfo);

            Assert.NotNull(resultProcess);
            Assert.Equal(dummyProcess, resultProcess);
        }

        [Fact]
        public void Play_ShouldThrowExecutableRunException_WhenFindExecFilePathReturnsNull()
        {
            var manager = CreateManager();
            var execInfo = CreateTestExecInfo();

            _execPathSystemMock.Setup(e => e.FindExecFilePath(execInfo)).Returns((string?)null);

            var exception = Assert.Throws<ExecutableRunException>(() => manager.Play(execInfo));
            Assert.IsType<FileNotFoundException>(exception.InnerException);
        }
    }
}