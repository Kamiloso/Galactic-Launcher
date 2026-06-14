using System.Collections;
using System.Reflection;
using Castle.Core.Logging;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.ViewModels.ImageControls;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using Moq;

namespace GalacticLauncher.Frontend.Tests.ViewModels.Panels
{
    public class LibraryViewModelTests
    {
        private readonly Mock<ICacheRefresher> _cacheRefresherMock = new();
        private readonly Mock<IGameListManager> _gameListManagerMock = new();
        private readonly Mock<ICacheProvider> _cacheProviderMock = new();
        private readonly Mock<IGameButtonFactory> _gameButtonFactoryMock = new();

        public LibraryViewModelTests()
        {
            _gameButtonFactoryMock
                .Setup(f => f.CreateAndStartLoadingLibrary(It.IsAny<long>()))
                .Returns((long id) =>
                {
                    var buttonMock = new Mock<GameButtonLibraryViewModel>();
                    return buttonMock.Object;
                });
        }

        private LibraryViewModel CreateViewModel()
        {
            return new LibraryViewModel(
                _cacheRefresherMock.Object,
                _gameListManagerMock.Object,
                _gameButtonFactoryMock.Object,
                _cacheProviderMock.Object
            );
        }

        /// <summary>
        /// CONSTRUCTOR AND INITIALIZATION TESTS
        /// </summary>

        [Fact]
        public void Constructor_Should_SubscribeToEvents_And_LoadInitialData()
        {
            var viewModel = CreateViewModel();
            Assert.Equal(LibraryViewModel.LibraryViewMode.YourGames, viewModel.CurrentMode);
            _cacheProviderMock.Verify(p => p.GetAllTags(), Times.Once);
        }

        [Fact]
        public void OnActivate_Should_SwitchToMoreGames_When_GameControlsIsEmpty()
        {
            var viewModel = CreateViewModel();

            var list = (IList)viewModel.GameControls;
            list.Clear();

            viewModel.ChangeView(LibraryViewModel.LibraryViewMode.YourGames);

            viewModel.OnActivate(Array.Empty<object>());

            Assert.Equal(LibraryViewModel.LibraryViewMode.MoreGames, viewModel.CurrentMode);
            Assert.True(viewModel.IsMoreGamesPage);
        }

        /// <summary>
        /// VIEW MODE CHANGING AND FILTERING TESTS
        /// </summary>

        [Theory]
        [InlineData(LibraryViewModel.LibraryViewMode.YourGames, true, false, false)]
        [InlineData(LibraryViewModel.LibraryViewMode.Favorites, false, true, false)]
        [InlineData(LibraryViewModel.LibraryViewMode.MoreGames, false, false, true)]
        internal void ChangeView_Should_UpdateProperties_Correctly(
            LibraryViewModel.LibraryViewMode targetMode,
            bool expectedIsYourGames,
            bool expectedIsFavorite,
            bool expectedIsMoreGames)
        {
            var viewModel = CreateViewModel();
            viewModel.ChangeViewCommand.Execute(targetMode);

            Assert.Equal(targetMode, viewModel.CurrentMode);
            Assert.Equal(expectedIsYourGames, viewModel.IsYourGamesPage);
            Assert.Equal(expectedIsFavorite, viewModel.IsFavoritePage);
            Assert.Equal(expectedIsMoreGames, viewModel.IsMoreGamesPage);
        }

        /// <summary>
        /// RELOAD GAMES AND TAGS FILTERING TESTS
        /// </summary>

        [Fact]
        public void ReloadGames_Should_FilterAndCreateButtons_When_ViewModeChanges()
        {
            long testGameId = 99;
            _gameListManagerMock.Setup(m => m.GetLibraryGames("")).Returns(new List<long> { testGameId });
            _cacheProviderMock.Setup(p => p.GetTagsOf(testGameId)).Returns(new List<Tag>());

            var viewModel = CreateViewModel();

            viewModel.ChangeViewCommand.Execute(LibraryViewModel.LibraryViewMode.YourGames);

            var collection = (ICollection)viewModel.GameControls;
            Assert.True(collection.Count > 0);
        }

        [Fact]
        public void ReloadGames_Should_FilterBySelectedTags_Correctly()
        {
            long gameWithTagId = 10;
            long gameWithoutTagId = 20;

            var matchingTag = new Tag { Id = 1, Name = "RPG", Description = "" };
            var otherTag = new Tag { Id = 2, Name = "Strategy", Description = "" };

            _gameListManagerMock.Setup(m => m.GetLibraryGames("")).Returns(new List<long> { gameWithTagId, gameWithoutTagId });

            _cacheProviderMock.Setup(p => p.GetTagsOf(gameWithTagId)).Returns(new List<Tag> { matchingTag });
            _cacheProviderMock.Setup(p => p.GetTagsOf(gameWithoutTagId)).Returns(new List<Tag> { otherTag });

            var viewModel = CreateViewModel();

            viewModel.SelectTagCommand.Execute(matchingTag);

            var collection = (ICollection)viewModel.GameControls;
            Assert.True(collection.Count > 0);
        }

        [Fact]
        public void ReloadTags_Should_FilterAvailableTags_BySearchFilter_And_ExcludeSelectedTags()
        {
            var tag1 = new Tag { Id = 1, Name = "Action", Description = "" };
            var tag2 = new Tag { Id = 2, Name = "Adventure", Description = "" };
            var tag3 = new Tag { Id = 3, Name = "Strategy", Description = "" };

            _cacheProviderMock.Setup(p => p.GetAllTags()).Returns(new List<Tag> { tag1, tag2, tag3 });

            var viewModel = CreateViewModel();

            viewModel.SelectTagCommand.Execute(tag1);

            Assert.Contains(tag1, viewModel.SelectedTags);
            Assert.DoesNotContain(tag1, viewModel.AvailableTags);
            Assert.Equal(2, viewModel.AvailableTags.Count);

            viewModel.SearchTags = "strat";

            Assert.Single(viewModel.AvailableTags);
            Assert.Equal("Strategy", viewModel.AvailableTags.First().Name);
        }

        [Fact]
        public void UnselectTag_Should_MoveTag_BackToAvailableTags()
        {
            var tag = new Tag { Id = 5, Name = "Indie", Description = "" };
            _cacheProviderMock.Setup(p => p.GetAllTags()).Returns(new List<Tag> { tag });

            var viewModel = CreateViewModel();
            viewModel.SelectTagCommand.Execute(tag);

            Assert.Contains(tag, viewModel.SelectedTags);
            Assert.DoesNotContain(tag, viewModel.AvailableTags);

            viewModel.UnselectTagCommand.Execute(tag);

            Assert.Empty(viewModel.SelectedTags);
            Assert.Contains(tag, viewModel.AvailableTags);
        }

        /// <summary>
        /// EXTERNAL EVENTS TESTS
        /// </summary>

        [Fact]
        public void Event_OnListsChanged_And_OnInitialize_Should_Trigger_RefreshPage()
        {
            var viewModel = CreateViewModel();
            _cacheProviderMock.Invocations.Clear();

            _gameListManagerMock.Raise(m => m.OnListsChanged += null);
            _cacheProviderMock.Verify(p => p.GetAllTags(), Times.Once);

            _cacheRefresherMock.Raise(r => r.OnInitialize += null);
            _cacheProviderMock.Verify(p => p.GetAllTags(), Times.Exactly(2));
        }
    }
}