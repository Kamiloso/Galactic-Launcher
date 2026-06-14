using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Services.Data;
using Moq;

namespace GalacticLauncher.Frontend.Tests.Services.Data
{
    public class LastGameManagerTests
    {
        private readonly Mock<ICacheRepository> _cacheRepositoryMock = new();
        private readonly Mock<IDataRepository> _dataRepositoryMock = new();
        private readonly LastGameManager _manager;

        public LastGameManagerTests()
        {
            _manager = new LastGameManager(_cacheRepositoryMock.Object, _dataRepositoryMock.Object);
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
        ///  GET LAST GAME TESTS
        /// </summary>

        [Fact]
        public void GetLastGame_SUCCESS_ShouldReturnId_WhenGameExistsInCache()
        {
            long expectedGameId = 42;
            var cachedGame = CreateMockGame(expectedGameId, "Galactic Invaders");

            _dataRepositoryMock.Setup(r => r.GetAll("last")).Returns(new List<long> { expectedGameId });
            _cacheRepositoryMock.Setup(r => r.GetGame(expectedGameId)).Returns(cachedGame);

            long? result = _manager.GetLastGame();

            Assert.NotNull(result);
            Assert.Equal(expectedGameId, result);
        }

        [Fact]
        public void GetLastGame_FAIL_ShouldReturnNull_WhenNoGameSavedInRepository()
        {
            _dataRepositoryMock.Setup(r => r.GetAll("last")).Returns(new List<long>());

            long? result = _manager.GetLastGame();

            Assert.Null(result);
            _cacheRepositoryMock.Verify(r => r.GetGame(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public void GetLastGame_FAIL_ShouldReturnNull_WhenSavedGameIsMissingInCache()
        {
            long savedGameId = 99;

            _dataRepositoryMock.Setup(r => r.GetAll("last")).Returns(new List<long> { savedGameId });
            _cacheRepositoryMock.Setup(r => r.GetGame(savedGameId)).Returns((Game?)null);

            long? result = _manager.GetLastGame();

            Assert.Null(result);
        }

        /// <summary>
        ///  SET LAST GAME TESTS
        /// </summary>

        [Fact]
        public void SetLastGame_WithValidId_ShouldClearPreviousAndAddNewId()
        {
            long newGameId = 77;

            _manager.SetLastGame(newGameId);

            _dataRepositoryMock.Verify(r => r.Clear("last"), Times.Once);
            _dataRepositoryMock.Verify(r => r.Add("last", newGameId), Times.Once);
        }

        [Fact]
        public void SetLastGame_WithNull_ShouldOnlyClearPreviousValue()
        {
            _manager.SetLastGame(null);

            _dataRepositoryMock.Verify(r => r.Clear("last"), Times.Once);
            _dataRepositoryMock.Verify(r => r.Add(It.IsAny<string>(), It.IsAny<long>()), Times.Never);
        }
    }
}