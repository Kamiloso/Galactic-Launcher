using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using Moq;

namespace GalacticLauncher.Frontend.Tests.Services.Data
{
    public class GameListManagerTests
    {
        private readonly Mock<IDataRepository> _dataRepositoryMock = new();
        private readonly Mock<ICacheProvider> _cacheProviderMock = new();
        private readonly GameListManager _manager;

        public GameListManagerTests()
        {
            _manager = new GameListManager(_dataRepositoryMock.Object, _cacheProviderMock.Object);
        }

        private Game CreateMockGame(long id, string name)
        {
            return new Game
            {
                Id = id,
                Name = name,
                Author = "Test",
                Description = "Test",
                IconUrl = null,
                TagIdList = null
            };
        }

        /// <summary>
        /// LIST RELATIONS TESTS
        /// </summary>

        [Fact]
        public void AddToFavorite_ShouldAlsoAddToLibrary_AndTriggerEvent()
        {
            // ARRANGE
            long gameId = 10;
            int eventTriggeredCount = 0;
            _manager.OnListsChanged += () => eventTriggeredCount++;

            // ACT
            _manager.AddToFavorite(gameId);

            // ASSERT
            _dataRepositoryMock.Verify(r => r.Add("library", gameId), Times.Once);
            _dataRepositoryMock.Verify(r => r.Add("favorites", gameId), Times.Once);
            Assert.Equal(1, eventTriggeredCount);
        }

        [Fact]
        public void RemoveFromLibrary_ShouldAlsoRemoveFromFavorite_AndTriggerEvent()
        {
            // ARRANGE
            long gameId = 20;
            int eventTriggeredCount = 0;
            _manager.OnListsChanged += () => eventTriggeredCount++;

            // ACT
            _manager.RemoveFromLibrary(gameId);

            // ASSERT
            _dataRepositoryMock.Verify(r => r.Remove("favorites", gameId), Times.Once);
            _dataRepositoryMock.Verify(r => r.Remove("library", gameId), Times.Once);
            Assert.Equal(1, eventTriggeredCount);
        }

        [Fact]
        public void RemoveFromFavorite_ShouldOnlyRemoveFromFavorite_AndLeaveLibraryIntact()
        {
            // ARRANGE
            long gameId = 30;
            int eventTriggeredCount = 0;
            _manager.OnListsChanged += () => eventTriggeredCount++;

            // ACT
            _manager.RemoveFromFavorite(gameId);

            // ASSERT
            _dataRepositoryMock.Verify(r => r.Remove("favorites", gameId), Times.Once);
            _dataRepositoryMock.Verify(r => r.Remove("library", gameId), Times.Never); // Nie ruszamy biblioteki!
            Assert.Equal(1, eventTriggeredCount);
        }

        /// <summary>
        /// FILTERING GAMES AND CLEANING TESTS
        /// </summary>


        [Fact]
        public void GetLibraryGames_ShouldReturnFilteredAndSortedGames_AndCleanUpMissingOnes()
        {
            // ARRANGE
            var cachedGames = new List<Game>
            {
                CreateMockGame(1, "Cyberpunk"),
                CreateMockGame(2, "Witcher")
            };

            var storedIds = new List<long> { 1, 2, 3 };

            _cacheProviderMock.Setup(c => c.GetAllGames()).Returns(cachedGames);
            _cacheProviderMock.Setup(c => c.GetGameOf(1)).Returns(cachedGames[0]);
            _cacheProviderMock.Setup(c => c.GetGameOf(2)).Returns(cachedGames[1]);
            _cacheProviderMock.Setup(c => c.GetGameOf(3)).Returns((Game?)null);

            _dataRepositoryMock.Setup(r => r.GetAll("library")).Returns(storedIds);

            var result = _manager.GetLibraryGames("cp").ToList();

            _dataRepositoryMock.Verify(r => r.Remove("library", 3), Times.Once);

            Assert.Single(result);
            Assert.Equal(1, result[0]);
        }

        [Fact]
        public void GetNolibGames_ShouldReturnOnlyGamesNotPresentInLibrary()
        {
            var allGames = new List<Game>
            {
                CreateMockGame(1, "Game 1"),
                CreateMockGame(2, "Game 2"),
                CreateMockGame(3, "Game 3")
            };
            var libraryIds = new List<long> { 2 };

            _cacheProviderMock.Setup(c => c.GetAllGames()).Returns(allGames);
            _cacheProviderMock.Setup(c => c.GetGameOf(1)).Returns(allGames[0]);
            _cacheProviderMock.Setup(c => c.GetGameOf(2)).Returns(allGames[1]);
            _cacheProviderMock.Setup(c => c.GetGameOf(3)).Returns(allGames[2]);

            _dataRepositoryMock.Setup(r => r.GetAll("library")).Returns(libraryIds);

            var result = _manager.GetNolibGames().ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(1, result);
            Assert.Contains(3, result);
            Assert.DoesNotContain(2, result);
        }


        /// <summary>
        /// RECOMMENDATIONS TESTS
        /// </summary>

        [Fact]
        public void InLibrary_ShouldReturnTrue_WhenRepositoryContainsId()
        {
            _dataRepositoryMock.Setup(r => r.GetAll("library")).Returns(new List<long> { 5, 10 });

            Assert.True(_manager.InLibrary(5));
            Assert.False(_manager.InLibrary(7));
        }

        [Fact]
        public void ObtainLibraryRecommendations_ShouldReturnLimitedAmountOfGames()
        {
            var allGames = new List<Game>
            {
                CreateMockGame(1, "A"), CreateMockGame(2, "B"), CreateMockGame(3, "C")
            };
            _cacheProviderMock.Setup(c => c.GetAllGames()).Returns(allGames);
            _cacheProviderMock.Setup(c => c.GetGameOf(It.IsAny<long>())).Returns((long id) => allGames.First(g => g.Id == id));
            _dataRepositoryMock.Setup(r => r.GetAll("library")).Returns(new List<long> { 1, 2, 3 });

            var result = _manager.ObtainLibraryRecommendations(2).ToList(); 

            Assert.Equal(2, result.Count);
        }
    }
}