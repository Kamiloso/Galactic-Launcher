using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Services.Executables;

namespace GalacticLauncher.Frontend.Tests.Services.Executables
{
    public class ExecPathSystemTests : IDisposable
    {
        private readonly List<string> _directoriesToClean = new();

        public ExecPathSystemTests()
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

        private ExecPathSystem CreateSystem()
        {
            return new ExecPathSystem();
        }

        private GameInfo CreateGameInfo(string gameUnique)
        {
            return new GameInfo
            {
                GameId = 1,
                GameName = "Test Game Name",
                GameUnique = gameUnique
            };
        }

        private ExecInfo CreateExecInfo(string gameUnique, string versionUnique, string execLocation)
        {
            return new ExecInfo
            {
                GameId = 1,
                GameName = "Test Game Name",
                GameUnique = gameUnique,

                VersionId = 10,
                VersionName = "v1.0.0",
                VersionUnique = versionUnique,
                CliArgs = "--fullscreen",
                DownloadUrl = "https://galactic-launcher.com/download/game.zip",
                ExecLocation = execLocation,
                Sha256Hash = "DUMMY_SHA256_HASH_VALUE"
            };
        }

        private void RegisterCleanupForGame(string gameUnique)
        {
            string fullGamePath = Path.Combine(Utils.RootPath, gameUnique);
            _directoriesToClean.Add(fullGamePath);
        }

        /// <summary>
        /// PATH BUILDER TESTS
        /// </summary>

        [Fact]
        public void PrepareGamePath_ShouldReturnCorrectPath_AndNotCreateDir_WhenEnsureIsFalse()
        {
            var system = CreateSystem();
            var gameInfo = CreateGameInfo("GalacticGame_X");
            string expectedPath = Path.Combine(Utils.RootPath, "GalacticGame_X");

            string result = system.PrepareGamePath(gameInfo, ensure: false);

            Assert.Equal(expectedPath, result);
            Assert.False(Directory.Exists(result), "Folder shouldn't be physically created on the disk.");
        }

        [Fact]
        public void PrepareGamePath_ShouldCreateDirectory_WhenEnsureIsTrue()
        {
            var system = CreateSystem();
            string gameUnique = "GalacticGame_ToCreate";
            var gameInfo = CreateGameInfo(gameUnique);
            string expectedPath = Path.Combine(Utils.RootPath, gameUnique);
            RegisterCleanupForGame(gameUnique);

            string result = system.PrepareGamePath(gameInfo, ensure: true);

            Assert.Equal(expectedPath, result);
            Assert.True(Directory.Exists(result), "Folder shouldn't be physically created on the disk.");
        }

        [Fact]
        public void PrepareExecPath_ShouldReturnCorrectCombinedPath()
        {
            var system = CreateSystem();
            var execInfo = CreateExecInfo("MyGame", "Version_Alpha", "game.exe");
            string expectedPath = Path.Combine(Utils.RootPath, "MyGame", "Version_Alpha");

            string result = system.PrepareExecPath(execInfo, ensure: false);

            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void PrepareInstancePath_ShouldReturnPathWithInstanceFolder()
        {
            var system = CreateSystem();
            var execInfo = CreateExecInfo("MyGame", "Version_Beta", "bin/start.exe");
            string expectedPath = Path.Combine(Utils.RootPath, "MyGame", "Version_Beta", "Instance");

            string result = system.PrepareInstancePath(execInfo, ensure: false);

            Assert.Equal(expectedPath, result);
        }

        /// <summary>
        /// FINDEXECFILEPATH TESTS
        /// </summary>


        [Fact]
        public void FindExecFilePath_ShouldReturnNull_WhenFileIsMissingOnDisk()
        {
            var system = CreateSystem();
            string gameUnique = "MissingGame";
            var execInfo = CreateExecInfo(gameUnique, "v1", "executable.exe");
            RegisterCleanupForGame(gameUnique);

            string? result = system.FindExecFilePath(execInfo);

            Assert.Null(result);
        }

        [Fact]
        public void FindExecFilePath_ShouldReturnFullPath_WhenExecutableFileExists()
        {
            var system = CreateSystem();
            string gameUnique = "ExistingGame";
            var execInfo = CreateExecInfo(gameUnique, "v2", "bin/game_launcher.exe");

            string instancePath = Path.Combine(Utils.RootPath, gameUnique, "v2", "Instance");
            string fullExecFilePath = Path.Combine(instancePath, "bin", "game_launcher.exe");

            Directory.CreateDirectory(Path.GetDirectoryName(fullExecFilePath)!);
            File.WriteAllText(fullExecFilePath, "mock binary data");

            RegisterCleanupForGame(gameUnique);

            string? result = system.FindExecFilePath(execInfo);

            Assert.NotNull(result);
            Assert.Equal(fullExecFilePath, result);
        }

        [Fact]
        public void FindExecFilePath_ShouldReturnNull_WhenPathTraversalAttackIsAttempted()
        {
            var system = CreateSystem();
            string gameUnique = "SecureGame";

            var maliciousExecInfo = CreateExecInfo(gameUnique, "v1", "../../../../unauthorized_file.exe");
            RegisterCleanupForGame(gameUnique);

            string? result = system.FindExecFilePath(maliciousExecInfo);

            Assert.Null(result);
        }
    }
}