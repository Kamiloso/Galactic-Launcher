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
    bool IsRefreshing { get; }

    event Action? OnRefreshAll;
    event Action<long>? OnRefreshGameData;

    Task RefreshAll();
    Task RefreshGameData(long id);
}

internal class CacheRefresher : ICacheRefresher
{
    public bool IsRefreshing => _refreshCount > 0;

    public event Action? OnRefreshAll;
    public event Action<long>? OnRefreshGameData;

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
            IEnumerable<Tag> tags;

            try
            {
                games = await _backendTalker.GetAllGames();
                tags = await _backendTalker.GetAllTags();

                _cacheRepository.UpdateMoreGames(games);
                _cacheRepository.OverwriteAllTags(tags);
            }
            catch (ApiException) { }

            OnRefreshAll?.Invoke();
        });

    public async Task RefreshGameData(long id) =>
        await DuringRefresh(async () =>
        {
            GameData gameData;

            try
            {
                gameData = (await _backendTalker.GetGameData(id))
                    .RemoveIncompatiblePlatforms();
                _cacheRepository.UpdateGame(gameData);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == 404) // not found - game probably removed
                {
                    _cacheRepository.ForgetGameEntry(id);
                }
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
