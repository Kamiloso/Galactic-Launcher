using GalacticLauncher.Core;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using Moq;

using GameVersion = GalacticLauncher.Core.Models.Version;

namespace GalacticLauncher.Frontend.Tests.ViewModels.Panels
{
    public enum ImageType { Banner, Screenshot }
    public record ImageItem(string DownloadUrl, ImageType Type, int SortIndex);
    public record GameData(List<ImageItem> Images);

    public class GameViewModelTests
    {
        private readonly Mock<ICacheProvider> _cacheProviderMock = new();
        private readonly Mock<ICacheRefresher> _cacheRefresherMock = new();
        private readonly Mock<IExecManager> _execManagerMock = new();
        private readonly Mock<IPreferenceManager> _preferenceManagerMock = new();
        private readonly Mock<IGamePlayService> _gamePlayServiceMock = new();
        private readonly Mock<IGameListManager> _gameListManagerMock = new();
        private readonly Mock<IImageFactory> _imageFactoryMock = new();
        private readonly Mock<ILastGameManager> _lastGameManagerMock = new();

        private GameViewModel CreateViewModel()
        {
            return new GameViewModel(
                _cacheProviderMock.Object,
                _cacheRefresherMock.Object,
                _execManagerMock.Object,
                _preferenceManagerMock.Object,
                _gamePlayServiceMock.Object,
                _gameListManagerMock.Object,
                _imageFactoryMock.Object,
                _lastGameManagerMock.Object
            );
        }

        /// <summary>
        ///  ON ACTIVATE AND INITIALIZATION TESTS
        /// </summary>

        [Fact]
        public void OnActivate_ShouldLoadBasicData_TriggerRefresh_AndLoadPreferences()
        {
            var viewModel = CreateViewModel();
            long testGameId = 42;

            var mockGame = new Game
            {
                Id = testGameId,
                Name = "Cyberpunk 2026",
                Description = "RPG",
                Author = "CDPR",
                IconUrl = "http://example.com/icon.png",
                TagIdList = "1"
            };

            var mockVersions = new List<GameVersion>
            {
                new GameVersion
                {
                    Id = 1,
                    IsPrimary = true,
                    Caption = "v1.0",
                    Type = default,
                    Description = "Initial release",
                    CliArgs = "",
                    ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                    Platform = default,
                    DownloadUrl = "http://example.com/v1",
                    ExecLocation = "game.exe",
                    Sha256Hash = null,
                    Alert = (AlertLevel)0
                },
                new GameVersion
                {
                    Id = 2,
                    IsPrimary = false,
                    Caption = "v1.1-beta",
                    Type = default,
                    Description = "Beta test",
                    CliArgs = "",
                    ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                    Platform = default,
                    DownloadUrl = "http://example.com/v2",
                    ExecLocation = "game_beta.exe",
                    Sha256Hash = null,
                    Alert = (AlertLevel)0
                }
            };

            var mockTags = new List<Tag>{new Tag{Id = 1,Name = "Sci-Fi", Description="description"}};

            _cacheProviderMock.Setup(p => p.GetGameOf(testGameId)).Returns(mockGame);
            _cacheProviderMock.Setup(p => p.GetVersionsOf(testGameId)).Returns(mockVersions);
            _cacheProviderMock.Setup(p => p.GetTagsOf(testGameId)).Returns(mockTags);

            _preferenceManagerMock.Setup(p => p.GetGameBool(testGameId, "ins-snapshot", true)).Returns(true);
            _preferenceManagerMock.Setup(p => p.GetGameBool(testGameId, "avb-snapshot", false)).Returns(false);

            viewModel.OnActivate([testGameId]);

            Assert.Equal(testGameId, viewModel.Id);
            Assert.Equal("Cyberpunk 2026", viewModel.Title);
            Assert.Equal("RPG", viewModel.Description);
            Assert.Equal("CDPR", viewModel.Author);

            _cacheRefresherMock.Verify(r => r.RefreshGameDataAsync(testGameId), Times.Once);
        }

        /// <summary>
        ///  GAME VERSION SEGREGATION TESTS
        /// </summary>

        [Fact]
        public void UpdateView_ShouldSeparateVersionsInto_InstalledAndAvailable()
        {
            var viewModel = CreateViewModel();
            long gameId = 7;

            var game = new Game
            {
                Id = gameId,
                Name = "Test Game",
                Description = "Test Desc",
                Author = "Test Author",
                IconUrl = null,
                TagIdList = null
            };

            var v1Installed = new GameVersion
            {
                Id = 101,
                IsPrimary = true,
                Caption = "v1",
                Type = default,
                Description = "",
                CliArgs = "",
                ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                Platform = default,
                DownloadUrl = "http://url",
                ExecLocation = "bin.exe",
                Sha256Hash = null,
                Alert = (AlertLevel)0
            };

            var v2Available = new GameVersion
            {
                Id = 102,
                IsPrimary = false,
                Caption = "v2",
                Type = default,
                Description = "",
                CliArgs = "",
                ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                Platform = default,
                DownloadUrl = "http://url",
                ExecLocation = "bin.exe",
                Sha256Hash = null,
                Alert = (AlertLevel)0
            };

            _cacheProviderMock.Setup(p => p.GetGameOf(gameId)).Returns(game);
            _cacheProviderMock.Setup(p => p.GetVersionsOf(gameId)).Returns(new List<GameVersion> { v1Installed, v2Available });

            _execManagerMock.Setup(m => m.Exists(It.Is<ExecInfo>(e => e.VersionId == 101))).Returns(true);
            _execManagerMock.Setup(m => m.Exists(It.Is<ExecInfo>(e => e.VersionId == 102))).Returns(false);

            viewModel.OnActivate([gameId]);

            Assert.Single(viewModel.InstalledVersions);
            Assert.Equal(101, viewModel.InstalledVersions.First().Id);

            Assert.Single(viewModel.AvailableVersions);
            Assert.Equal(102, viewModel.AvailableVersions.First().Id);
        }

        /// <summary>
        ///  RELAY COMMANDS TESTS
        /// </summary>

        [Fact]
        public async Task DownloadSelectedVersionCommand_ShouldCallService_AndAddToLibrary_WhenSuccessful()
        {
            var viewModel = CreateViewModel();
            long gameId = 12;

            var game = new Game
            {
                Id = gameId,
                Name = "Test Game",
                Description = "",
                Author = "",
                IconUrl = null,
                TagIdList = null
            };

            var version = new GameVersion
            {
                Id = 50,
                IsPrimary = true,
                Caption = "v1",
                Type = default,
                Description = "",
                CliArgs = "",
                ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                Platform = default,
                DownloadUrl = "http://url",
                ExecLocation = "bin.exe",
                Sha256Hash = null,
                Alert = (AlertLevel)0
            };

            _cacheProviderMock.Setup(p => p.GetGameOf(gameId)).Returns(game);
            _cacheProviderMock.Setup(p => p.GetVersionsOf(gameId)).Returns(new List<GameVersion> { version });

            _execManagerMock.Setup(m => m.Exists(It.IsAny<ExecInfo>())).Returns(false);
            _gamePlayServiceMock.Setup(s => s.Download(It.IsAny<ExecInfo>())).ReturnsAsync(true);

            viewModel.OnActivate([gameId]);

            await viewModel.DownloadSelectedVersionCommand.ExecuteAsync(null);

            _gamePlayServiceMock.Verify(s => s.Download(It.IsAny<ExecInfo>()), Times.Once);
            _gameListManagerMock.Verify(m => m.AddToLibrary(gameId), Times.Once);
        }

        [Fact]
        public async Task PlaySelectedVersionCommand_ShouldSetLastGame_WhenExecutionIsSuccessful()
        {
            var viewModel = CreateViewModel();
            long gameId = 99;

            var game = new Game
            {
                Id = gameId,
                Name = "Test Game",
                Description = "",
                Author = "",
                IconUrl = null,
                TagIdList = null
            };

            var version = new GameVersion
            {
                Id = 1,
                IsPrimary = true,
                Caption = "v1",
                Type = default,
                Description = "",
                CliArgs = "",
                ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                Platform = default,
                DownloadUrl = "http://url",
                ExecLocation = "bin.exe",
                Sha256Hash = null,
                Alert = (AlertLevel)0
            };

            _cacheProviderMock.Setup(p => p.GetGameOf(gameId)).Returns(game);
            _cacheProviderMock.Setup(p => p.GetVersionsOf(gameId)).Returns(new List<GameVersion> { version });

            _execManagerMock.Setup(m => m.Exists(It.IsAny<ExecInfo>())).Returns(true);
            _gamePlayServiceMock.Setup(s => s.PlayAndTerminate(It.IsAny<ExecInfo>())).ReturnsAsync(true);

            viewModel.OnActivate([gameId]);

            await viewModel.PlaySelectedVersionCommand.ExecuteAsync(null);

            _gamePlayServiceMock.Verify(s => s.PlayAndTerminate(It.IsAny<ExecInfo>()), Times.Once);
            _lastGameManagerMock.Verify(m => m.SetLastGame(gameId), Times.Once);
        }

        [Fact]
        public void ToggleFavoriteCommand_ShouldAddGame_WhenCurrentlyNotInFavorites()
        {
            var viewModel = CreateViewModel();
            long gameId = 55;

            var game = new Game
            {
                Id = gameId,
                Name = "Test Game",
                Description = "",
                Author = "",
                IconUrl = null,
                TagIdList = null
            };
            _cacheProviderMock.Setup(p => p.GetGameOf(gameId)).Returns(game);

            _gameListManagerMock.Setup(m => m.InFavorite(gameId)).Returns(false);
            viewModel.OnActivate([gameId]);

            viewModel.ToggleFavoriteCommand.Execute(null);

            _gameListManagerMock.Verify(m => m.AddToFavorite(gameId), Times.Once);
        }

        [Fact]
        public void EventOnListsChanged_ShouldRaisePropertyChanged_ForLibraryAndFavoriteFlags()
        {
            var viewModel = CreateViewModel();
            bool inLibraryRaised = false;
            bool inFavoriteRaised = false;

            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(viewModel.InLibrary)) inLibraryRaised = true;
                if (e.PropertyName == nameof(viewModel.InFavorite)) inFavoriteRaised = true;
            };

            _gameListManagerMock.Raise(m => m.OnListsChanged += null);

            Assert.True(inLibraryRaised);
            Assert.True(inFavoriteRaised);
        }
    }
}