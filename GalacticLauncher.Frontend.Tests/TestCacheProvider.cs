using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Services.Cache;
using Moq;

using Version = GalacticLauncher.Core.Models.Version;

namespace GalacticLauncher.Frontend.Tests.Services.Cache
{
    public class CacheProviderTests
    {
        private readonly Mock<ICacheRepository> _cacheRepositoryMock = new();
        private readonly CacheProvider _service;

        public CacheProviderTests()
        {
            _service = new CacheProvider(_cacheRepositoryMock.Object);
        }

        [Fact]
        public void GetGameOf_ShouldReturnGame_WhenGameExistsInRepository()
        {
            var expectedGame = new Game
            {
                Id = 1,
                Name = "Witcher 3",
                Author = "CDPR",
                Description = "RPG Game",
                IconUrl = "http://icon.png",
                TagIdList = "1,2"
            };
            _cacheRepositoryMock.Setup(r => r.GetGame(1)).Returns(expectedGame);

            var result = _service.GetGameOf(1);

            Assert.NotNull(result);
            Assert.Equal("Witcher 3", result.Name);
        }

        [Fact]
        public void GetGameDataOf_ShouldReturnGameData_WhenGameIsInstanceOfGameData()
        {
            var expectedGameData = new GameData
            {
                Id = 1,
                Name = "Cyberpunk 2077",
                Author = "CDPR",
                Description = "Sci-Fi RPG",
                IconUrl = "http://cp.png",
                TagIdList = "1",
                Versions = [],
                Images = [] 
            };
            _cacheRepositoryMock.Setup(r => r.GetGame(1)).Returns(expectedGameData);

            var result = _service.GetGameDataOf(1);

            Assert.NotNull(result);
            Assert.IsType<GameData>(result);
            Assert.Equal("Cyberpunk 2077", result.Name);
        }

        [Fact]
        public void GetGameDataOf_ShouldReturnNull_WhenGameIsNotGameData()
        {
            var regularGame = new Game
            {
                Id = 2,
                Name = "Simple Tetris",
                Author = "Unknown",
                Description = "Blocks",
                IconUrl = null,
                TagIdList = null
            };
            _cacheRepositoryMock.Setup(r => r.GetGame(2)).Returns(regularGame);

            var result = _service.GetGameDataOf(2);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllGames_ShouldReturnMappedListOfGames_BasedOnRepositoryIds()
        {
            var ids = new List<long> { 10, 20 };
            var game1 = new Game { Id = 10, Name = "Game A", Author = "Auth A", Description = "Desc A", IconUrl = null, TagIdList = null };
            var game2 = new Game { Id = 20, Name = "Game B", Author = "Auth B", Description = "Desc B", IconUrl = null, TagIdList = null };

            _cacheRepositoryMock.Setup(r => r.GetAllGames()).Returns(ids);
            _cacheRepositoryMock.Setup(r => r.GetGame(10)).Returns(game1);
            _cacheRepositoryMock.Setup(r => r.GetGame(20)).Returns(game2);

            var result = _service.GetAllGames().ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, g => g.Id == 10);
            Assert.Contains(result, g => g.Id == 20);
        }

        [Fact]
        public void GetVersionsOf_ShouldReturnVersionsOrderedByReleaseDateDescending()
        {
            var versionOld = CreateMockVersion(101, new DateOnly(2026, 1, 1));
            var versionNew = CreateMockVersion(102, new DateOnly(2026, 6, 1));

            var gameData = new GameData
            {
                Id = 1,
                Name = "Minecraft",
                Author = "Mojang",
                Description = "Sandbox",
                IconUrl = null,
                TagIdList = null,
                Versions = [versionOld, versionNew],
                Images = []
            };

            _cacheRepositoryMock.Setup(r => r.GetGame(1)).Returns(gameData);

            var result = _service.GetVersionsOf(1).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal(102, result[0].Id);
            Assert.Equal(101, result[1].Id);
        }

        [Fact]
        public void GetVersionsOf_ShouldReturnEmpty_WhenGameDoesNotExistOrIsNotGameData()
        {
            _cacheRepositoryMock.Setup(r => r.GetGame(5)).Returns((Game?)null);

            var result = _service.GetVersionsOf(5);

            Assert.Empty(result);
        }

        [Fact]
        public void GetAllTags_ShouldReturnAllAvailableTagsFromRepository()
        {
            var tagIds = new List<long> { 1, 2 };
            var tag1 = new Tag { Id = 1, Name = "RPG", Description = "Role Playing Game" };
            var tag2 = new Tag { Id = 2, Name = "Action", Description = "Fast paced" };

            _cacheRepositoryMock.Setup(r => r.GetAllTags()).Returns(tagIds);
            _cacheRepositoryMock.Setup(r => r.GetTag(1)).Returns(tag1);
            _cacheRepositoryMock.Setup(r => r.GetTag(2)).Returns(tag2);

            var result = _service.GetAllTags().ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("RPG", result[0].Name);
        }

        [Fact]
        public void GetTagsOf_ShouldReturnMappedTags_WhenGameHasTagIdList()
        {
            var game = new Game
            {
                Id = 1,
                Name = "Starcraft",
                Author = "Blizzard",
                Description = "RTS",
                IconUrl = null,
                TagIdList = "1,3"
            };

            var allTagIds = new List<long> { 1, 2, 3 };
            var tag1 = new Tag { Id = 1, Name = "Strategy", Description = "RTS" };
            var tag2 = new Tag { Id = 2, Name = "Indie", Description = "Independent" };
            var tag3 = new Tag { Id = 3, Name = "Sci-Fi", Description = "Space setting" };

            _cacheRepositoryMock.Setup(r => r.GetGame(1)).Returns(game);
            _cacheRepositoryMock.Setup(r => r.GetAllTags()).Returns(allTagIds);
            _cacheRepositoryMock.Setup(r => r.GetTag(1)).Returns(tag1);
            _cacheRepositoryMock.Setup(r => r.GetTag(2)).Returns(tag2);
            _cacheRepositoryMock.Setup(r => r.GetTag(3)).Returns(tag3);

            var result = _service.GetTagsOf(1).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, t => t.Name == "Strategy");
            Assert.Contains(result, t => t.Name == "Sci-Fi");
            Assert.DoesNotContain(result, t => t.Name == "Indie");
        }

        private static Version CreateMockVersion(long id, DateOnly releaseDate)
        {
            return new Version
            {
                Id = id,
                Caption = $"Build {id}",
                Description = "Test version release",
                CliArgs = "-nogui",
                IsPrimary = true,
                ReleaseDate = releaseDate,
                DownloadUrl = "https://galactic.com/file.zip",
                ExecLocation = "bin/game.exe",
                Sha256Hash = null,
                Type = default,
                Platform = default,
                Alert = default
            };
        }
    }
}