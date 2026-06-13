using NSubstitute;
using NSubstitute.ExceptionExtensions;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Domain.Exceptions;
using GalacticLauncher.Backend.Domain.Models.Extensions;
using GalacticLauncher.Backend.Tests.Helpers;

namespace GalacticLauncher.Backend.Tests.Services;

public class DataUpdateServiceTests : ServiceTestBase
{
    private readonly IGameTreeWriter _gameTreeWriter;
    private readonly IGameRepository _gameRepo;
    private readonly ITagRepository _tagRepo;
    private readonly DataUpdateService _sut;

    public DataUpdateServiceTests()
    {
        _gameTreeWriter = Substitute.For<IGameTreeWriter>();
        _gameRepo = Substitute.For<IGameRepository>();
        _tagRepo = Substitute.For<ITagRepository>();

        _scope.GetService<IGameTreeWriter>().Returns(_gameTreeWriter);
        _scope.GetService<IGameRepository>().Returns(_gameRepo);
        _scope.GetService<ITagRepository>().Returns(_tagRepo);

        _sut = new DataUpdateService(_scopeFactory);
    }

    [Fact]
    public async Task UpdateGameTree_Success_CommitsTransaction()
    {
        var dummyTree = TestDataHelper.CreateDummyGameTree();
        
        _gameTreeWriter.ReplaceGameData(
            Arg.Is<GameEntity>(e => 
                e.Id == dummyTree.Id && 
                e.Name == dummyTree.Name && 
                e.Author == dummyTree.Author && 
                e.Description == dummyTree.Description), 
            Arg.Is<IEnumerable<VersionEntity>>(v => !v.Any()), 
            Arg.Is<IEnumerable<ImageEntity>>(i => !i.Any()), 
            Arg.Is<IEnumerable<long>>(t => t.Count() == 1))
            .Returns(true);

        await _sut.UpdateGameTree(dummyTree);

        await _scope.Received(1).CommitAsync();
    }

    [Fact]
    public async Task UpdateGameTree_WriterFails_ThrowsBadRequestException()
    {
        var dummyTree = TestDataHelper.CreateDummyGameTree();
        
        _gameTreeWriter.ReplaceGameData(
            Arg.Is<GameEntity>(e => e.Id == dummyTree.Id), 
            Arg.Any<IEnumerable<VersionEntity>>(), 
            Arg.Any<IEnumerable<ImageEntity>>(), 
            Arg.Any<IEnumerable<long>>())
            .Returns(false);

        var ex = await Assert.ThrowsAsync<ClientFaultException>(
            () => _sut.UpdateGameTree(dummyTree));

        Assert.Equal(400, ex.StatusCode);
        await _scope.DidNotReceive().CommitAsync();
    }

    [Fact]
    public async Task CreateGame_Success_ReturnsIdAndCommits()
    {
        var expectedId = 150L;
        var dummyRaw = TestDataHelper.CreateDummyGameRaw();
        
        _gameRepo.CreateGame(Arg.Is<GameEntity>(e => 
                     e.Name == dummyRaw.Name && 
                     e.Author == dummyRaw.Author && 
                     e.Description == dummyRaw.Description))
                 .Returns(expectedId);

        long result = await _sut.CreateGame(dummyRaw);

        Assert.Equal(expectedId, result);
        await _scope.Received(1).CommitAsync();
    }

    [Fact]
    public async Task CreateGame_RepositoryThrowsException_ThrowsExceptionWithoutCommitting()
    {
        var dummyRaw = TestDataHelper.CreateDummyGameRaw();
        var expectedException = new InvalidOperationException("DB Failure");

        _gameRepo.CreateGame(Arg.Any<GameEntity>()).Throws(expectedException);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateGame(dummyRaw));

        Assert.Equal("DB Failure", ex.Message);
        await _scope.DidNotReceive().CommitAsync();
    }

    [Fact]
    public async Task DeleteGameById_Success_Commits()
    {
        var targetId = 5L;
        await _sut.DeleteGameById(targetId);

        await _gameRepo.Received(1).DeleteGameById(targetId);
        await _scope.Received(1).CommitAsync();
    }

    [Fact]
    public async Task CreateTag_Success_ReturnsIdAndCommits()
    {
        var expectedId = 42L;
        var dummyTag = TestDataHelper.CreateDummyTag();
        
        _tagRepo.CreateTag(Arg.Is<TagEntity>(t => 
                    t.Name == dummyTag.Name && 
                    t.Description == dummyTag.Description))
                .Returns(expectedId);

        long result = await _sut.CreateTag(dummyTag);

        Assert.Equal(expectedId, result);
        await _scope.Received(1).CommitAsync();
    }

    [Fact]
    public async Task DeleteTagById_Success_Commits()
    {
        var targetId = 7L;
        await _sut.DeleteTagById(targetId);

        await _tagRepo.Received(1).DeleteTagById(targetId);
        await _scope.Received(1).CommitAsync();
    }
    
    [Fact]
    public async Task UpdateGameTree_Success_PassesChildrenAndCommitsTransaction()
    {
        var dummyVersion = TestDataHelper.CreateDummyVersionEntity().ToDomain();
        var dummyImage = TestDataHelper.CreateDummyImageEntity().ToDomain();
        
        var dummyTree = TestDataHelper.CreateDummyGameTree() with 
        {
            Versions = [dummyVersion],
            Images = [dummyImage],
            TagIds = [5, 10]
        };
        
        _gameTreeWriter.ReplaceGameData(
                Arg.Any<GameEntity>(), 
                Arg.Any<IEnumerable<VersionEntity>>(), 
                Arg.Any<IEnumerable<ImageEntity>>(), 
                Arg.Any<IEnumerable<long>>())
            .Returns(true);

        await _sut.UpdateGameTree(dummyTree);

        await _scope.Received(1).CommitAsync();
        
        await _gameTreeWriter.Received(1).ReplaceGameData(
            Arg.Is<GameEntity>(e => e.Id == dummyTree.Id), 
            Arg.Is<IEnumerable<VersionEntity>>(v => v.Select(x => x.Id).SequenceEqual(new[] { dummyVersion.Id })), 
            Arg.Is<IEnumerable<ImageEntity>>(i => i.Select(x => x.Id).SequenceEqual(new[] { dummyImage.Id })), 
            Arg.Is<IEnumerable<long>>(t => t.OrderBy(x => x).SequenceEqual(new[] { 5L, 10L })));
    }
}