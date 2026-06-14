using GalacticLauncher.Core;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Executables;
using Moq;

using GameVersion = GalacticLauncher.Core.Models.Version;

namespace GalacticLauncher.Frontend.Tests.Services.Executables
{
    public class ExecCleanerTests : IDisposable
    {
        private readonly Mock<ICacheProvider> _cacheProviderMock = new();
        private readonly Mock<ICacheRefresher> _cacheRefresherMock = new();
        private readonly Mock<IExecPathSystem> _execPathSystemMock = new();

        private readonly List<string> _directoriesToClean = new();

        public ExecCleanerTests()
        {
        }

        public void Dispose()
        {
            foreach (var dir in _directoriesToClean)
            {
                if (Directory.Exists(dir))
                {
                    try { Directory.Delete(dir, true); } catch { }
                }
            }
        }

        private ExecCleaner CreateCleaner()
        {
            return new ExecCleaner(
                _cacheProviderMock.Object,
                _cacheRefresherMock.Object,
                _execPathSystemMock.Object
            );
        }

        private Game CreateTestGame(long id, string name)
        {
            return new Game
            {
                Id = id,
                Name = name,
                Author = "FakeAuthor",
                Description = "FakeDescription",

                IconUrl = "http://fakeurl.com/icon.png",
                TagIdList = "1,2,3"
            };
        }

        private GameVersion CreateTestVersion(long id)
        {
            return new GameVersion
            {
                Id = id,
                Caption = "Version " + id,
                Type = default, 
                Description = "Fake Description",
                CliArgs = "",
                IsPrimary = true,
                ReleaseDate = DateOnly.FromDateTime(DateTime.Today),
                Platform = default,
                DownloadUrl = "http://fakeurl.com/download",
                ExecLocation = "bin/game.exe",
                Sha256Hash = null,
                Alert = default 
            };
        }

        private string CreateTestDirectory(string baseRoot, string folderName)
        {
            string fullPath = Path.Combine(baseRoot, folderName);
            Directory.CreateDirectory(fullPath);
            _directoriesToClean.Add(fullPath);
            return fullPath;
        }

        [Fact]
        public void CleanAllGames_ShouldDeleteDirectory_WhenGameIsMissingInCache()
        {
            var cleaner = CreateCleaner();
            string orphanDirPath = CreateTestDirectory(Utils.RootPath, "Game_OldOrphanedGame");

            _cacheProviderMock.Setup(c => c.GetAllGames()).Returns(new List<Game>());

            _cacheRefresherMock.Raise(r => r.OnBaseRefresh += null);

            Assert.False(Directory.Exists(orphanDirPath), "Orphaned catalog should have been deleted.");
        }

        [Fact]
        public void CleanAllGames_ShouldKeepDirectory_WhenGameExistsInCache()
        {
            var cleaner = CreateCleaner();
            var activeGame = CreateTestGame(123, "GalacticWar");
            string activeGamePath = CreateTestDirectory(Utils.RootPath, "Game_Active");

            _cacheProviderMock.Setup(c => c.GetAllGames()).Returns(new List<Game> { activeGame });

            _execPathSystemMock.Setup(e => e.PrepareGamePath(It.IsAny<GameInfo>(), false))
                               .Returns(activeGamePath);

            _cacheRefresherMock.Raise(r => r.OnBaseRefresh += null);

            Assert.True(Directory.Exists(activeGamePath), "Catalog of an active game should have been deleted.");
        }

        [Fact]
        public void CleanGameVersions_ShouldDoNothing_WhenGameNotFoundInCache()
        {
            var cleaner = CreateCleaner();
            long gameId = 999;

            _cacheProviderMock.Setup(c => c.GetGameOf(gameId)).Returns((Game?)null!);

            _cacheRefresherMock.Raise(r => r.OnRefreshGameData += null, gameId);

            _cacheProviderMock.Verify(c => c.GetVersionsOf(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public void CleanGameVersions_ShouldDeleteOldVersionDirectory_WhenNotPresentInCache()
        {
            var cleaner = CreateCleaner();
            long gameId = 1;
            var game = CreateTestGame(gameId, "TestGame");

            string gameRootPath = CreateTestDirectory(Utils.RootPath, "Game_1");
            string oldVersionPath = CreateTestDirectory(gameRootPath, "Version_1.0.0_Old");

            _cacheProviderMock.Setup(c => c.GetGameOf(gameId)).Returns(game);
            _cacheProviderMock.Setup(c => c.GetVersionsOf(gameId)).Returns(new List<GameVersion>().AsEnumerable());

            _execPathSystemMock.Setup(e => e.PrepareGamePath(It.IsAny<GameInfo>(), false))
                               .Returns(gameRootPath);

            _cacheRefresherMock.Raise(r => r.OnRefreshGameData += null, gameId);

            Assert.False(Directory.Exists(oldVersionPath), "Old version of the game should have been deleted.");
        }

        [Fact]
        public void CleanGameVersions_ShouldKeepVersionDirectory_WhenVersionIsActiveInCache()
        {
            var cleaner = CreateCleaner();
            long gameId = 1;
            var game = CreateTestGame(gameId, "TestGame");
            var activeVersion = CreateTestVersion(55);

            string gameRootPath = CreateTestDirectory(Utils.RootPath, "Game_1");
            string activeVersionPath = CreateTestDirectory(gameRootPath, "Version_55_Active");

            _cacheProviderMock.Setup(c => c.GetGameOf(gameId)).Returns(game);
            _cacheProviderMock.Setup(c => c.GetVersionsOf(gameId)).Returns(new List<GameVersion> { activeVersion }.AsEnumerable());

            _execPathSystemMock.Setup(e => e.PrepareGamePath(It.IsAny<GameInfo>(), false))
                               .Returns(gameRootPath);

            _execPathSystemMock.Setup(e => e.PrepareExecPath(It.IsAny<ExecInfo>(), false))
                               .Returns(activeVersionPath);

            _cacheRefresherMock.Raise(r => r.OnRefreshGameData += null, gameId);

            Assert.True(Directory.Exists(activeVersionPath), "Current game version should be unchanged.");
        }
    }
}