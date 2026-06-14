using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Tools.Networking;
using Moq;

namespace GalacticLauncher.Frontend.Tests.Services.Cache
{
    public class CacheRefresherTests
    {
        private readonly Mock<IBackendTalker> _backendTalkerMock = new();
        private readonly Mock<ICacheRepository> _cacheRepositoryMock = new();
        private readonly Mock<IErrorHandler> _errorHandlerMock = new();
        private readonly CacheRefresher _service;

        public CacheRefresherTests()
        {
            _service = new CacheRefresher(
                _backendTalkerMock.Object,
                _cacheRepositoryMock.Object,
                _errorHandlerMock.Object);
        }

        [Fact]
        public async Task RefreshRootAsync_SUCCESS_ShouldUpdateCacheAndTriggerEvents()
        {
            var mockGames = new List<Game> { new Game { Id = 1, Name = "Game 1", Author = "A", Description = "D", IconUrl = null, TagIdList = null } };
            var mockTags = new List<Tag> { new Tag { Id = 1, Name = "Tag 1", Description = "D" } };

            _backendTalkerMock.Setup(b => b.GetAllGames()).ReturnsAsync(mockGames);
            _backendTalkerMock.Setup(b => b.GetAllTags()).ReturnsAsync(mockTags);

            int initEventCount = 0;
            int baseRefreshEventCount = 0;

            _service.OnInitialize += () => initEventCount++;
            _service.OnBaseRefresh += () => baseRefreshEventCount++;

            await _service.RefreshRootAsync();

            Assert.True(_service.Initialized);
            Assert.Equal(1, initEventCount);
            Assert.Equal(1, baseRefreshEventCount);

            _cacheRepositoryMock.Verify(r => r.UpdateMoreGames(mockGames, true), Times.Once);
            _cacheRepositoryMock.Verify(r => r.OverwriteAllTags(mockTags), Times.Once);
        }

        [Fact]
        public async Task RefreshRootAsync_SUCCESS_AlreadyInitialized_ShouldNotTriggerOnInitializeAgain()
        {
            _backendTalkerMock.Setup(b => b.GetAllGames()).ReturnsAsync(new List<Game>());
            _backendTalkerMock.Setup(b => b.GetAllTags()).ReturnsAsync(new List<Tag>());

            await _service.RefreshRootAsync();
            Assert.True(_service.Initialized);

            int initEventCount = 0;
            int baseRefreshEventCount = 0;
            _service.OnInitialize += () => initEventCount++;
            _service.OnBaseRefresh += () => baseRefreshEventCount++;

            await _service.RefreshRootAsync();

            Assert.Equal(0, initEventCount); 
            Assert.Equal(1, baseRefreshEventCount); 
        }

        [Fact]
        public async Task RefreshRootAsync_EXCEPTION_ApiError_ShouldHandleExceptionAndStillComplete()
        {
            var apiException = new ApiException("No connection", 503);
            _backendTalkerMock.Setup(b => b.GetAllGames()).ThrowsAsync(apiException);

            int baseRefreshEventCount = 0;
            _service.OnBaseRefresh += () => baseRefreshEventCount++;

            await _service.RefreshRootAsync();

            _errorHandlerMock.Verify(e => e.HandleApiError(503, true), Times.Once);
            Assert.Equal(1, baseRefreshEventCount);
        }

        [Fact]
        public async Task RefreshGameDataAsync_SUCCESS_ShouldUpdateGameAndTriggerEvent()
        {
            long gameId = 42;
            var mockGameData = new GameData
            {
                Id = gameId,
                Name = "Space Game",
                Author = "Galactic",
                Description = "Cool",
                IconUrl = null,
                TagIdList = null,
                Versions = [],
                Images = []
            };

            _backendTalkerMock.Setup(b => b.GetGameData(gameId)).ReturnsAsync(mockGameData);

            long? triggeredGameId = null;
            _service.OnRefreshGameData += (id) => triggeredGameId = id;

            await _service.RefreshGameDataAsync(gameId);

            Assert.Equal(gameId, triggeredGameId);
            _cacheRepositoryMock.Verify(r => r.UpdateGame(It.Is<GameData>(g => g.Id == gameId)), Times.Once);
        }

        [Fact]
        public async Task RefreshGameDataAsync_EXCEPTION_ApiError_ShouldHandleErrorWithNoInternetFalse()
        {
            long gameId = 99;
            var apiException = new ApiException("Not Found", 404);
            _backendTalkerMock.Setup(b => b.GetGameData(gameId)).ThrowsAsync(apiException);

            await _service.RefreshGameDataAsync(gameId);

            _errorHandlerMock.Verify(e => e.HandleApiError(404, false), Times.Once);
            _cacheRepositoryMock.Verify(r => r.UpdateGame(It.IsAny<GameData>()), Times.Never);
        }

        [Fact]
        public async Task IsRefreshing_ShouldBeTrue_DuringActiveRefreshTask()
        {
            var tcs = new TaskCompletionSource<IEnumerable<Game>>();

            _backendTalkerMock.Setup(b => b.GetAllGames()).Returns(tcs.Task);
            _backendTalkerMock.Setup(b => b.GetAllTags()).ReturnsAsync(new List<Tag>());

            Task refreshTask = _service.RefreshRootAsync();

            bool isRefreshingDuringTask = _service.IsRefreshing;

            tcs.SetResult(new List<Game>());
            await refreshTask;

            Assert.True(isRefreshingDuringTask);
            Assert.False(_service.IsRefreshing);
        }
    }
}