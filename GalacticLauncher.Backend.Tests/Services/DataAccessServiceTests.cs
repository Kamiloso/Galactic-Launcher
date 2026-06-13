using NSubstitute;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Domain.Exceptions;
using GalacticLauncher.Backend.Tests.Helpers;

namespace GalacticLauncher.Backend.Tests.Services;

public class DataAccessServiceTests : ServiceTestBase
{
    private readonly IGameRepository _gameRepo;
    private readonly IVersionRepository _versionRepo;
    private readonly IImageRpository _imageRepo;
    private readonly ITagRepository _tagRepo;
    private readonly DataAccessService _sut;

    public DataAccessServiceTests()
    {
        _gameRepo = Substitute.For<IGameRepository>();
        _versionRepo = Substitute.For<IVersionRepository>();
        _imageRepo = Substitute.For<IImageRpository>();
        _tagRepo = Substitute.For<ITagRepository>();

        _scope.GetService<IGameRepository>().Returns(_gameRepo);
        _scope.GetService<IVersionRepository>().Returns(_versionRepo);
        _scope.GetService<IImageRpository>().Returns(_imageRepo);
        _scope.GetService<ITagRepository>().Returns(_tagRepo);

        _sut = new DataAccessService(_scopeFactory);
    }

    [Fact]
    public async Task GetGameDataById_NotFound_ThrowsClientFaultException()
    {
        var searchId = 99L;
        _gameRepo.GetGameById(searchId).Returns((GamePlusEntity?)null);

        var ex = await Assert.ThrowsAsync<ClientFaultException>(
            () => _sut.GetGameDataById(searchId));
            
        Assert.Contains("not found", ex.Message);
        Assert.Equal(404, ex.StatusCode);
    }

    [Fact]
    public async Task GetGameDataById_Exists_ReturnsMappedData()
    {
        var expectedId = 42L;
        var dummyEntity = TestDataHelper.CreateDummyGamePlusEntity() with { Id = expectedId };
        
        _gameRepo.GetGameById(expectedId).Returns(dummyEntity);

        var dummyVersions = new List<VersionEntity> { TestDataHelper.CreateDummyVersionEntity() };
        var dummyImages = new List<ImageEntity> { TestDataHelper.CreateDummyImageEntity() };

        _versionRepo.GetVersionsByGameId(expectedId).Returns(dummyVersions);
        _imageRepo.GetImagesByGameId(expectedId).Returns(dummyImages);

        var result = await _sut.GetGameDataById(expectedId);

        Assert.NotNull(result);
        Assert.Equal(expectedId, result.Id); 
        Assert.Equal(dummyEntity.Name, result.Name);
    }

    [Fact]
    public async Task GetAllGames_ReturnsMappedData()
    {
        var dummyEntity = TestDataHelper.CreateDummyGamePlusEntity();
        var dummyList = new List<GamePlusEntity> { dummyEntity };
        
        _gameRepo.GetAllGames().Returns(dummyList);

        var result = (await _sut.GetAllGames()).ToList();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(dummyEntity.Id, result.First().Id);
    }

    [Fact]
    public async Task GetAllTags_ReturnsMappedData()
    {
        var dummyTag = TestDataHelper.CreateDummyTagEntity();
        var dummyList = new List<TagEntity> { dummyTag };
        
        _tagRepo.GetAllTags().Returns(dummyList);

        var result = (await _sut.GetAllTags()).ToList();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(dummyTag.Id, result.First().Id);
    }
    
    [Fact]
    public async Task GetGameDataById_Exists_ReturnsMappedDataWithChildren()
    {
        var expectedId = 42L;
        var dummyEntity = TestDataHelper.CreateDummyGamePlusEntity() with { Id = expectedId };
        
        _gameRepo.GetGameById(expectedId).Returns(dummyEntity);

        var dummyVersions = new List<VersionEntity> { TestDataHelper.CreateDummyVersionEntity() };
        var dummyImages = new List<ImageEntity> { TestDataHelper.CreateDummyImageEntity() };

        _versionRepo.GetVersionsByGameId(expectedId).Returns(dummyVersions);
        _imageRepo.GetImagesByGameId(expectedId).Returns(dummyImages);

        var result = await _sut.GetGameDataById(expectedId);

        Assert.NotNull(result);
        Assert.Equal(expectedId, result.Id); 
        Assert.Equal(dummyEntity.Name, result.Name);
        
        Assert.Single(result.Versions);
        Assert.Equal(dummyVersions[0].Id, result.Versions.First().Id);
        
        Assert.Single(result.Images);
        Assert.Equal(dummyImages[0].Id, result.Images.First().Id);
    }

    [Fact]
    public async Task GetAllGames_EmptyDatabase_ReturnsEmptyList()
    {
        _gameRepo.GetAllGames().Returns(new List<GamePlusEntity>());

        var result = await _sut.GetAllGames();

        Assert.NotNull(result);
        Assert.Empty(result); 
    }
}