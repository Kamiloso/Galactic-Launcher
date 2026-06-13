using GalacticLauncher.Backend.Domain.Exceptions;
using NSubstitute;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Backend.Domain.Models;

namespace GalacticLauncher.Backend.Tests.Services;

public class HistoryServiceTests : ServiceTestBase
{
    private readonly IHistoryRepository _historyRepo;
    private readonly HistoryService _sut;

    public HistoryServiceTests()
    {
        _historyRepo = Substitute.For<IHistoryRepository>();
        _scope.GetService<IHistoryRepository>().Returns(_historyRepo);

        _sut = new HistoryService(_scopeFactory, _fakeConfig);
    }

    [Fact]
    public async Task LogToHistory_StringOverload_CallsRepositoryAddLog_AndCommits()
    {
        var expectedLog = "Action executed";
        var expectedGameId = 123L;

        await _sut.LogToHistory(expectedLog, expectedGameId);

        await _historyRepo.Received(1).AddLog(Arg.Is<HistoryEntity>(entity => 
            entity.Info == expectedLog && 
            entity.IdGame == expectedGameId
        ));

        await _scope.Received(1).CommitAsync();
    }

    [Fact]
    public async Task GetHistoryEntries_ValidPage_ReturnsDomainList()
    {
        var mockEntities = new List<HistoryEntity>
        {
            new() { Id = 1, Info = "Log 1", Timestamp = DateTime.UtcNow, IdGame = null },
            new() { Id = 2, Info = "Log 2", Timestamp = DateTime.UtcNow, IdGame = null }
        };

        var targetPage = 1;
        _historyRepo.GetHistoryEntries(targetPage, _fakeConfig.History.PageSize)
            .Returns(mockEntities);

        var result = (await _sut.GetHistoryEntries(targetPage)).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Log 1", result[0].Info);
    }
}