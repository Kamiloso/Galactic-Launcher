using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.Tools.Networking;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using Moq;

namespace GalacticLauncher.Frontend.Tests.Services.Executables
{
    public class GamePlayServiceTests
    {
        private readonly Mock<IExecManager> _execManagerMock = new();
        private readonly Mock<ITelemetryCollector> _telemetryMock = new();
        private readonly Mock<IDialogs> _dialogsMock = new();
        private readonly Mock<INotifications> _notificationsMock = new();
        private readonly Mock<ITerminator> _terminatorMock = new();

        private readonly GamePlayService _service;

        public GamePlayServiceTests()
        {
            _service = new GamePlayService(
                _execManagerMock.Object,
                _telemetryMock.Object,
                _dialogsMock.Object,
                _notificationsMock.Object,
                _terminatorMock.Object);
        }

        private ExecInfo CreateValidExecInfo()
        {
            return new ExecInfo
            {
                GameId = 1,
                GameName = "Space Adventure",
                GameUnique = "space-adventure-unique",
                VersionId = 42,
                VersionName = "v1.2.3",
                VersionUnique = "v1-2-3-unique",
                CliArgs = "--launch-direct",
                DownloadUrl = "https://launcher.galactic.com/download/game.zip",
                ExecLocation = "C:/GalacticGames/SpaceAdventure/game.exe",
                Sha256Hash = "a3f5b721...ef92"
            };
        }

        /// <summary>
        /// PLAY AND TERMINATE TESTS
        /// </summary>

        [Fact]
        public async Task PlayAndTerminate_SUCCESS_ShouldLaunchGameTrackTelemetryAndTerminate()
        {
            var execInfo = CreateValidExecInfo();
            _execManagerMock.Setup(m => m.Exists(execInfo)).Returns(true);
            _execManagerMock.Setup(m => m.Play(execInfo));

            bool result = await _service.PlayAndTerminate(execInfo);

            Assert.True(result);
            _telemetryMock.Verify(t => t.TrackGameLaunch(execInfo), Times.Once);
            _terminatorMock.Verify(t => t.Terminate(), Times.Once);
        }

        [Fact]
        public async Task PlayAndTerminate_FAIL_GameDoesNotExist_ShouldReturnFalseImmediately()
        {
            var execInfo = CreateValidExecInfo();
            _execManagerMock.Setup(m => m.Exists(execInfo)).Returns(false);

            bool result = await _service.PlayAndTerminate(execInfo);

            Assert.False(result);
            _dialogsMock.Verify(d => d.ShowLoadingDialogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Task>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task PlayAndTerminate_EXCEPTION_GameCrash_ShouldShowErrorNotificationAndReturnFalse()
        {
            var execInfo = CreateValidExecInfo();
            _execManagerMock.Setup(m => m.Exists(execInfo)).Returns(true);
            _execManagerMock.Setup(m => m.Play(execInfo)).Throws(new ExecutableRunException("DirectX Error"));

            bool result = await _service.PlayAndTerminate(execInfo);

            Assert.False(result);
            _notificationsMock.Verify(n => n.ShowError("Run Error", "DirectX Error"), Times.Once);
            _terminatorMock.Verify(t => t.Terminate(), Times.Never);
        }

        /// <summary>
        /// DELETE TESTS
        /// </summary>

        [Fact]
        public async Task Delete_SUCCESS_UserConfirmed_ShouldDeleteGameAndShowSuccess()
        {
            var execInfo = CreateValidExecInfo();
            _dialogsMock.Setup(d => d.ShowConfirmationDialogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(true);
            _execManagerMock.Setup(m => m.Exists(execInfo)).Returns(true);

            bool result = await _service.Delete(execInfo);

            Assert.True(result);
            _execManagerMock.Verify(m => m.Delete(execInfo), Times.Once);

            _notificationsMock.Verify(n => n.ShowSuccess("Version Deleted", "Space Adventure v1.2.3 has been deleted."), Times.Once);
        }

        [Fact]
        public async Task Delete_FAIL_UserCanceled_ShouldReturnFalseAndNotDeleteAnything()
        {
            var execInfo = CreateValidExecInfo();
            _dialogsMock.Setup(d => d.ShowConfirmationDialogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(false);

            bool result = await _service.Delete(execInfo);

            Assert.False(result);
            _execManagerMock.Verify(m => m.Delete(execInfo), Times.Never);
        }

        /// <summary>
        /// DOWNLOAD TESTS
        /// </summary>

        [Fact]
        public async Task Download_SUCCESS_GameNotInstalled_ShouldDownloadSuccessfully()
        {
            var execInfo = CreateValidExecInfo();
            _execManagerMock.Setup(m => m.Exists(execInfo)).Returns(false);

            bool result = await _service.Download(execInfo);

            Assert.True(result);

            _dialogsMock.Verify(d => d.ShowDownloadProgressDialogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Task>(),
                It.IsAny<Action>(),
                It.IsAny<Progress<DownloadProgressData>>()), Times.Once);
        }

        [Fact]
        public async Task Download_SUCCESS_AlreadyExists_ShouldReturnTrueImmediatelyWithoutDownloading()
        {
            var execInfo = CreateValidExecInfo();
            _execManagerMock.Setup(m => m.Exists(execInfo)).Returns(true);

            bool result = await _service.Download(execInfo);

            Assert.True(result);

            _dialogsMock.Verify(d => d.ShowDownloadProgressDialogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Task>(),
                It.IsAny<Action>(),
                It.IsAny<Progress<DownloadProgressData>>()), Times.Never);
        }

        [Fact]
        public async Task Download_EXCEPTION_NetworkError_ShouldShowErrorNotificationAndReturnFalse()
        {
            var execInfo = CreateValidExecInfo();
            _execManagerMock.Setup(m => m.Exists(execInfo)).Returns(false);

            _dialogsMock.Setup(d => d.ShowDownloadProgressDialogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Task>(),
                It.IsAny<Action>(),
                It.IsAny<Progress<DownloadProgressData>>()))
                .ThrowsAsync(new DownloadException("Download for Space Adventure v1.2.3 has failed."));

            bool result = await _service.Download(execInfo);

            Assert.False(result);

            _notificationsMock.Verify(n => n.ShowError("Download Error", "Download for Space Adventure v1.2.3 has failed."), Times.Once);
        }
    }
}