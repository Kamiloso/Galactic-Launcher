using GalacticLauncher.Backend.Domain.Models.Extensions;
using GalacticLauncher.Backend.Tests.Helpers;
using GalacticLauncher.Core;
using GalacticLauncher.Core.Models;
using Version = GalacticLauncher.Core.Models.Version;

namespace GalacticLauncher.Backend.Tests.Domain.Models.Extensions;

public class ToEntityConvertersTests
{
    [Fact]
    public void GameTree_ToEntityDeconstruct_PropagatesGameIdToChildren()
    {
        var dummyVersion = new Version 
        { 
            Id = 1, 
            Caption = "v1.0",
            Type = VersionType.Release,
            Description = "Test",
            CliArgs = "",
            IsPrimary = true,
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now),
            Platform = Platform.Windows,
            DownloadUrl = "http://test",
            ExecLocation = "run.exe",
            Sha256Hash = null,
            Alert = AlertLevel.Stable
        };

        var dummyImage = new Image 
        { 
            Id = 2, 
            DownloadUrl = "http://test",
            Type = ImageType.Screenshot,
            SortIndex = 0
        };

        var tree = TestDataHelper.CreateDummyGameTree() with
        {
            Versions = [dummyVersion],
            Images = [dummyImage],
            TagIds = [10, 20]
        };

        var (gameEntity, versions, images, tagIds) = tree.ToEntityDeconstruct();

        Assert.Equal(tree.Id, gameEntity.Id);
        
        Assert.Single(versions);
        Assert.Equal(tree.Id, versions.First().IdGame); 
        
        Assert.Single(images);
        Assert.Equal(tree.Id, images.First().IdGame); 
        
        Assert.Equal(2, tagIds.Count());
        Assert.Contains(10L, tagIds);
    }

    [Fact]
    public void GameRaw_ToEntity_MapsPropertiesCorrectly()
    {
        var raw = TestDataHelper.CreateDummyGameRaw();

        var entity = raw.ToEntity();

        Assert.Equal(raw.Id, entity.Id);
        Assert.Equal(raw.Name, entity.Name);
        Assert.Equal(raw.Author, entity.Author);
        Assert.Equal(raw.Description, entity.Description);
    }

    [Fact]
    public void Version_ToEntity_ConvertsEnumsToLowerCaseInvariantStrings()
    {
        var version = new Version
        {
            Id = 1,
            Caption = "v1.0",
            Type = VersionType.Release,
            Description = "Test",
            CliArgs = "",
            IsPrimary = true,
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now),
            Platform = Platform.Windows,
            DownloadUrl = "http://test",
            ExecLocation = "run.exe",
            Sha256Hash = null,
            Alert = AlertLevel.Danger
        };
        
        var parentId = 99L;

        var entity = version.ToEntity(parentId);

        Assert.Equal("release", entity.Type);
        Assert.Equal("windows", entity.Platform);
        Assert.Equal("danger", entity.Alert);
        Assert.Equal(parentId, entity.IdGame);
    }

    [Fact]
    public void Version_ToEntity_InvalidEnum_FallsBackToDefaultString()
    {
        var version = new Version
        {
            Id = 1,
            Caption = "v1.0",
            Type = (VersionType)999,
            Description = "Test",
            CliArgs = "",
            IsPrimary = true,
            ReleaseDate = DateOnly.FromDateTime(DateTime.Now),
            Platform = Platform.Windows,
            DownloadUrl = "http://test",
            ExecLocation = "run.exe",
            Sha256Hash = null,
            Alert = AlertLevel.Stable
        };

        var entity = version.ToEntity(1);

        Assert.Equal(default(VersionType).ToString().ToLowerInvariant(), entity.Type);
    }
}