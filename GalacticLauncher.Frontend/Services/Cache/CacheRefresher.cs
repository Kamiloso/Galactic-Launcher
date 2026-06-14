using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Tools.Networking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Services.Cache;

public interface ICacheRefresher
{
    bool Initialized { get; }
    bool IsRefreshing { get; }

    event Action? OnInitialize;
    event Action? OnBaseRefresh;
    event Action<long>? OnRefreshGameData;

    Task RefreshRootAsync();
    Task RefreshGameDataAsync(long id);
}

public class CacheRefresher(
    IBackendTalker backendTalker,
    ICacheRepository cacheRepository,
    IErrorHandler errorHandler) : ICacheRefresher
{
    public bool Initialized { get; private set; }
    public bool IsRefreshing => _refreshCount > 0;

    public event Action? OnInitialize;
    public event Action? OnBaseRefresh;
    public event Action<long>? OnRefreshGameData;

    private int _refreshCount;

    public async Task RefreshRootAsync() =>
        await DuringRefresh(async () =>
        {
            IEnumerable<Game> games;
            IEnumerable<Tag> tags;

            try
            {
                games = await backendTalker.GetAllGames();
                tags = await backendTalker.GetAllTags();

                cacheRepository.UpdateMoreGames(games, clearOther: true);
                cacheRepository.OverwriteAllTags(tags);
            }
            catch (ApiException ex)
            {
                errorHandler.HandleApiError(
                    ex.StatusCode, showNoInternet: true);
            }

            if (!Initialized)
            {
                Initialized = true;
                OnInitialize?.Invoke();
            }

            OnBaseRefresh?.Invoke();
        });

    public async Task RefreshGameDataAsync(long id) =>
        await DuringRefresh(async () =>
        {
            GameData gameData;

            try
            {
                gameData = (await backendTalker.GetGameData(id))
                    .RemoveIncompatiblePlatforms();

                cacheRepository.UpdateGame(gameData);
            }
            catch (ApiException ex)
            {
                errorHandler.HandleApiError(
                    ex.StatusCode, showNoInternet: false);
            }

            OnRefreshGameData?.Invoke(id);
        });

    private async Task DuringRefresh(Func<Task> task)
    {
        _refreshCount++;

        try
        {
            await task.Invoke();
        }
        finally
        {
            _refreshCount--;
        }
    }
}
