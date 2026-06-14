using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Services.Data;
using Moq;

namespace GalacticLauncher.Frontend.Tests.Services.Data
{
    public class PreferenceManagerTests
    {
        private readonly Mock<IMemoryRepository> _memoryRepositoryMock = new();

        private PreferenceManager CreateManager()
        {
            return new PreferenceManager(_memoryRepositoryMock.Object);
        }

        /// <summary>
        /// CONSTRUCTORS
        /// </summary>

        [Fact]
        public void Constructor_ShouldGenerateNewGuid_WhenGuidIsMissingInRepository()
        {
            _memoryRepositoryMock.Setup(r => r["guid"]).Returns((string)null!);

            var manager = CreateManager();

            Assert.NotEqual(Guid.Empty, manager.Guid);
            _memoryRepositoryMock.VerifySet(r => r["guid"] = It.IsAny<string>(), Times.Once);
        }

        [Fact]
        public void Constructor_ShouldReuseExistingGuid_WhenGuidExistsInRepository()
        {
            var existingGuid = Guid.NewGuid();
            _memoryRepositoryMock.Setup(r => r["guid"]).Returns(existingGuid.ToString());

            var manager = CreateManager();

            Assert.Equal(existingGuid, manager.Guid);
            _memoryRepositoryMock.VerifySet(r => r["guid"] = It.IsAny<string>(), Times.Never);
        }

        /// <summary>
        ///  THEME AND MENU TESTS
        /// </summary>

        [Theory]
        [InlineData("galactic", true)]
        [InlineData("blue", false)]
        [InlineData("unknown_value", true)]
        [InlineData(null, true)]
        public void IsThemeGalactic_Get_ShouldReturnExpectedValue_BasedOnRepositoryState(string? repoValue, bool expectedResult)
        {
            _memoryRepositoryMock.Setup(r => r["galactic"]).Returns(repoValue!);
            var manager = CreateManager();

            bool result = manager.IsThemeGalactic;

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void IsThemeGalactic_Set_ShouldStoreCorrectStringInRepository()
        {
            var manager = CreateManager();

            manager.IsThemeGalactic = true;
            _memoryRepositoryMock.VerifySet(r => r["galactic"] = "galactic", Times.Once);

            manager.IsThemeGalactic = false;
            _memoryRepositoryMock.VerifySet(r => r["galactic"] = "blue", Times.Once);
        }

        [Theory]
        [InlineData("expanded", true)]
        [InlineData("shrinked", false)]
        [InlineData(null, true)]
        public void IsMenuExpanded_Get_ShouldReturnExpectedValue(string? repoValue, bool expectedResult)
        {
            _memoryRepositoryMock.Setup(r => r["expanded"]).Returns(repoValue!);
            var manager = CreateManager();

            Assert.Equal(expectedResult, manager.IsMenuExpanded);
        }

        [Theory]
        [InlineData("visible", true)]
        [InlineData("hidden", false)]
        [InlineData(null, false)]
        public void IsAdminPanelVisible_Get_ShouldReturnExpectedValue(string? repoValue, bool expectedResult)
        {
            _memoryRepositoryMock.Setup(r => r["admin-panel"]).Returns(repoValue!);
            var manager = CreateManager();

            Assert.Equal(expectedResult, manager.IsAdminPanelVisible);
        }

        /// <summary>
        ///  GAME VERSION TESTS
        /// </summary>

        [Fact]
        public void GetSelectedVersion_ShouldReturnLong_WhenValidIdInRepository()
        {
            long gameId = 1;
            _memoryRepositoryMock.Setup(r => r["sel-version-1"]).Returns("105");
            var manager = CreateManager();

            long? versionId = manager.GetSelectedVersion(gameId);

            Assert.Equal(105, versionId);
        }

        [Fact]
        public void GetSelectedVersion_ShouldReturnNull_WhenValueIsInvalidOrEmpty()
        {
            long gameId = 2;
            _memoryRepositoryMock.Setup(r => r["sel-version-2"]).Returns("not_a_number");
            var manager = CreateManager();

            long? versionId = manager.GetSelectedVersion(gameId);

            Assert.Null(versionId);
        }

        [Fact]
        public void SetSelectedVersion_WithNull_ShouldStoreEmptyString()
        {
            long gameId = 3;
            var manager = CreateManager();

            manager.SetSelectedVersion(gameId, null);

            _memoryRepositoryMock.VerifySet(r => r["sel-version-3"] = "", Times.Once);
        }

        [Fact]
        public void GetGameBool_ShouldReturnDefaultValue_WhenKeyDoesNotExist()
        {
            long gameId = 10;
            string filterName = "IntroSkip";
            _memoryRepositoryMock.Setup(r => r["filter-10-IntroSkip"]).Returns((string)null!);
            var manager = CreateManager();

            bool result = manager.GetGameBool(gameId, filterName, defaultValue: true);

            Assert.True(result);
        }

        [Fact]
        public void GetGameBool_ShouldReturnParsedValue_WhenKeyExists()
        {
            long gameId = 10;
            string filterName = "IntroSkip";
            _memoryRepositoryMock.Setup(r => r["filter-10-IntroSkip"]).Returns("False");
            var manager = CreateManager();

            bool result = manager.GetGameBool(gameId, filterName, defaultValue: true);

            Assert.False(result);
        }
    }
}