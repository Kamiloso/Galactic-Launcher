using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Tools.Networking;
using GalacticLauncher.Frontend.Repositories;

namespace GalacticLauncher.Frontend.Services.Cache;

public interface ICacheRefresher
{
    bool IsRefreshing { get; }

    event Action? OnRefreshAll;
    event Action<long>? OnRefreshGame;

    Task RefreshAll();
    Task RefreshGame(long id);
}

internal class CacheRefresher(
    IBackendTalker backendTalker,
    ICacheRepository cacheRepository) : ICacheRefresher
{
    public bool IsRefreshing => _refreshCount > 0;

    public event Action? OnRefreshAll;
    public event Action<long>? OnRefreshGame;

    private int _refreshCount;

    public async Task RefreshAll() =>
        await DuringRefresh(async () =>
        {
            IEnumerable<Game> games;

            try
            {
                games = await backendTalker.GetAllGames();
                cacheRepository.SetAllGames(games);
            }
            catch (ApiException) { }

            OnRefreshAll?.Invoke();
        });

    public async Task RefreshGame(long id) =>
        await DuringRefresh(async () =>
        {
            GameData gameData;

            try
            {
                gameData = (await backendTalker.GetGameData(id))
                    .RemoveIncompatiblePlatforms();
                cacheRepository.SetGameData(gameData);
            }
            catch (ApiException ex)
            {

                if (ex.StatusCode == 404) // not found - game probably removed
                {
                    cacheRepository.ForgetGameEntry(id);
                }
            }

            OnRefreshGame?.Invoke(id);
        });

    private async Task DuringRefresh(Func<Task> task)
    {
        _refreshCount++;

        try
        {
            await task();
        }
        finally
        {
            _refreshCount--;
        }
    }
}
