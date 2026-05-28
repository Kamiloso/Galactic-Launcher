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
    event Action? OnRefreshTags;

    Task RefreshAll();
    Task RefreshGame(long id);
    Task RefreshTags();
}

internal class CacheRefresher : ICacheRefresher
{
    public bool IsRefreshing => _refreshCount > 0;

    public event Action? OnRefreshAll;
    public event Action<long>? OnRefreshGame;
    public event Action? OnRefreshTags;

    private int _refreshCount;

    private readonly IBackendTalker _backendTalker;
    private readonly ICacheRepository _cacheRepository;

    public CacheRefresher(
        IBackendTalker backendTalker,
        ICacheRepository cacheRepository)
    {
        _backendTalker = backendTalker;
        _cacheRepository = cacheRepository;

        _ = RefreshAll();
    }

    public async Task RefreshAll() =>
        await DuringRefresh(async () =>
        {
            IEnumerable<Game> games;

            try
            {
                games = await _backendTalker.GetAllGames();
                _cacheRepository.SetAllGames(games);
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
                gameData = (await _backendTalker.GetGameData(id))
                    .RemoveIncompatiblePlatforms();
                _cacheRepository.SetGameData(gameData);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == 404) // not found - game probably removed
                {
                    _cacheRepository.ForgetGameEntry(id);
                }
            }

            OnRefreshGame?.Invoke(id);
        });

    public async Task RefreshTags() =>
        await DuringRefresh(async () =>
        {
            IEnumerable<Tag> tags;
            try
            {
                tags = await _backendTalker.GetAllTags();
                _cacheRepository.SetAllTags(tags);
            }
            catch (ApiException) { }

            OnRefreshTags?.Invoke();
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
