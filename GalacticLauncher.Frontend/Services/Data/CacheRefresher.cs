using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Tools.Networking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Services.Data;

public interface ICacheRefresher
{
    bool Initialized { get; }
    bool IsRefreshing { get; }

    event Action? OnInitialize;
    event Action<long>? OnRefreshGameData;
    event Action<string, string>? OnError;

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
    public event Action<string, string>? OnError;

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
            catch (ApiException ex)
            {
                var(title, message) = ex.StatusCode switch
                {
                    0 or 408 => ("Offline Mode", "Could not reach the server. Loading local library."),
                    401 or 403 => (null, null),
                    404 => ("API Error", "Could not find the library endpoint. Make sure your launcher is up to date."),
                    >= 500 => ("Server Issues", "The Galactic Launcher servers are currently down. Playing offline."),
                    _ => ("Sync Warning", $"Could not update library (Code: {ex.StatusCode}).")
                };

                if (title != null && message != null)
                {
                    OnError?.Invoke(title, message);
                }
            }

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
            catch (ApiException ex)
            {
                var (title, message) = ex.StatusCode switch
                {
                    0 or 204 or 404 or 408 or 401 or 403 => (null, null),
                    429 => ("Too Many Requests", "You are refreshing too fast. Please wait a moment."),
                    >= 500 => ("Server Error", "Our servers are having trouble right now. Try again later."),
                    _ => ("Sync Error", $"An unexpected error occurred (Code: {ex.StatusCode}).")
                };

                if (title != null && message != null)
                {
                    OnError?.Invoke(title, message);
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
