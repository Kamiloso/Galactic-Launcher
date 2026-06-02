using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Domain.Models.Extensions;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Infrastructure.DbScopes;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Core.Models;
using System.Data;

namespace GalacticLauncher.Backend.Services;

public interface IHistoryService
{
    Task LogToHistory(string info, long? idGame = null);
    Task LogToHistory(History history);
    Task<IEnumerable<History>> GetHistoryEntries(int page);
}

internal class HistoryService(
    ILogger<HistoryService> logger,
    IAppScopeFactory scopeFactory,
    AppConfig config) : IHistoryService
{
    private readonly int MAX_ENTRIES = config.History.MaxEntries;
    private readonly int PAGE_SIZE = config.History.PageSize;

    private readonly DateTime _startDate = DateTime.Now;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(config.History.CleanupIntervalSeconds);

    private long _cleanCycle = 0;

    public async Task LogToHistory(string info, long? idGame = null)
    {
        await LogToHistory(new History
        {
            Id = 0, // auto_increment
            Info = info,
            Timestamp = DateTime.UtcNow,
            IdGame = idGame,
        });
    }

    public async Task LogToHistory(History history)
    {
        FireCleaningOperation();

        HistoryEntity historyEntity = history.ToEntity();

        await using var scope = await scopeFactory.CreateScopeAsync(
            IsolationLevel.RepeatableRead);

        var historyRepository = scope.GetService<IHistoryRepository>();

        await historyRepository.AddLog(historyEntity);
        await scope.CommitAsync();
    }

    public async Task<IEnumerable<History>> GetHistoryEntries(int page)
    {
        FireCleaningOperation();

        await using var scope = await scopeFactory.CreateScopeAsync(
            IsolationLevel.RepeatableRead);

        var historyRepository = scope.GetService<IHistoryRepository>();

        return [.. (await historyRepository.GetHistoryEntries(page, PAGE_SIZE))
            .Select(h => h.ToDomain())];
    }

    public void FireCleaningOperation()
    {
        _ = CustomThreading.LogTaskErrors(Task.Run(CheckAndCleanOld),
            "Error running the history cleaning operation",
            logger);

        async Task CheckAndCleanOld()
        {
            DateTime nowTime = DateTime.Now;

            long currentCycle = Interlocked.Read(ref _cleanCycle);

            long cleanTimeTicks = _startDate.Ticks + (_interval.Ticks * currentCycle);
            long cyclesPassed = (nowTime.Ticks - cleanTimeTicks) / _interval.Ticks;

            if (cyclesPassed > 0)
            {
                long targetCycle = currentCycle + cyclesPassed;

                if (Interlocked.CompareExchange(ref _cleanCycle, targetCycle, currentCycle) == currentCycle)
                {
                    await using var scope = await scopeFactory.CreateScopeAsync(
                        IsolationLevel.RepeatableRead);

                    var historyRepository = scope.GetService<IHistoryRepository>();

                    await historyRepository.ReduceHistoryTo(MAX_ENTRIES);
                    await scope.CommitAsync();
                }
            }
        }
    }
}
