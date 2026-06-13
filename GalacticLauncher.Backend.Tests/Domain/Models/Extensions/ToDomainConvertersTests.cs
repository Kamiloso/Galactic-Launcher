using GalacticLauncher.Backend.Domain.Models.Extensions;
using GalacticLauncher.Backend.Tests.Helpers;
using GalacticLauncher.Core;

namespace GalacticLauncher.Backend.Tests.Domain.Models.Extensions;

public class ToDomainConvertersTests
{
    [Fact]
    public void GamePlusEntity_ToDomain_MapsPropertiesCorrectly()
    {
        var entity = TestDataHelper.CreateDummyGamePlusEntity();

        var domain = entity.ToDomain();

        Assert.Equal(entity.Id, domain.Id);
        Assert.Equal(entity.Name, domain.Name);
        Assert.Equal(entity.Author, domain.Author);
        Assert.Equal(entity.Description, domain.Description);
        Assert.Equal(entity.IconUrl, domain.IconUrl);
        Assert.Equal(entity.TagIdList, domain.TagIdList);
    }

    [Fact]
    public void GamePlusEntity_ToGameData_MapsEntityAndChildLists()
    {
        var gameEntity = TestDataHelper.CreateDummyGamePlusEntity();
        var versionEntities = new[] { TestDataHelper.CreateDummyVersionEntity() };
        var imageEntities = new[] { TestDataHelper.CreateDummyImageEntity() };

        var gameData = gameEntity.ToDomain(versionEntities, imageEntities);

        Assert.Equal(gameEntity.Id, gameData.Id);
        Assert.Single(gameData.Versions);
        Assert.Single(gameData.Images);
        Assert.Equal(versionEntities[0].Id, gameData.Versions.First().Id);
        Assert.Equal(imageEntities[0].Id, gameData.Images.First().Id);
    }

    [Theory]
    [InlineData("release", VersionType.Release)]
    [InlineData("RELEASE", VersionType.Release)]
    [InlineData("beta", VersionType.Beta)]
    [InlineData("invalid_type", default(VersionType))]
    public void VersionEntity_ToDomain_ParsesEnumsCorrectly(string inputType, VersionType expectedType)
    {
        var entity = TestDataHelper.CreateDummyVersionEntity() with 
        { 
            Type = inputType
        };

        var domain = entity.ToDomain();

        Assert.Equal(expectedType, domain.Type);
    }

    [Fact]
    public void ImageEntity_ToDomain_MapsPropertiesCorrectly()
    {
        var entity = TestDataHelper.CreateDummyImageEntity() with 
        { 
            Type = "icon" 
        };

        var domain = entity.ToDomain();

        Assert.Equal(entity.Id, domain.Id);
        Assert.Equal(entity.DownloadUrl, domain.DownloadUrl);
        Assert.Equal(ImageType.Icon, domain.Type);
        Assert.Equal(entity.SortIndex, domain.SortIndex);
    }
}