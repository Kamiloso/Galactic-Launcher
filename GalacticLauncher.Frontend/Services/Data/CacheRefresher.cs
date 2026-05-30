using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Tools.Networking;
using GalacticLauncher.Frontend.Repositories;

namespace GalacticLauncher.Frontend.Services.Data;

public interface ICacheRefresher
{
    bool Initialized { get; }
    bool IsRefreshing { get; }

    event Action? OnInitialize;
    event Action<long>? OnRefreshGameData;

    Task InitializeAsync();
    Task RefreshGameDataAsync(long id);
}

internal class CacheRefresher(
    IBackendTalker backendTalker,
    ICacheRepository cacheRepository) : ICacheRefresher
{
    public bool Initialized { get; private set; }
    public bool IsRefreshing => _refreshCount > 0;

    public event Action? OnInitialize;
    public event Action<long>? OnRefreshGameData;

    private bool _initialized;
    private int _refreshCount;

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        _initialized = true;

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
            catch (ApiException) { }

            Initialized = true;

            OnInitialize?.Invoke();
        });
    }

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
            catch (ApiException) { }

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
