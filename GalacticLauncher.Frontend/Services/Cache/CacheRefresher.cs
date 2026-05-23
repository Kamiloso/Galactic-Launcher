using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.DataAccess.Networking;
using GalacticLauncher.Frontend.DataAccess.Repositories;

namespace GalacticLauncher.Frontend.Services.Cache;

public interface ICacheRefresher
{
    event Action? OnRefreshAllGames;
    event Action<long>? OnRefreshGame;
    Task RefreshAllGames();
    Task RefreshGame(long id);
}

internal class CacheRefresher(
    IBackendTalker backendTalker,
    ICacheRepository cacheRepository) : ICacheRefresher
{
    public event Action? OnRefreshAllGames;
    public event Action<long>? OnRefreshGame;

    public async Task RefreshAllGames()
    {
        IEnumerable<Game> games;

        try
        {
            games = await backendTalker.GetAllGames();
            cacheRepository.SaveAllGames(games);
        }
        catch (ApiException) { }

        OnRefreshAllGames?.Invoke();
    }

    public async Task RefreshGame(long id)
    {
        GameData gameData;

        try
        {
            gameData = (await backendTalker.GetGameData(id))
                .RemoveIncompatiblePlatforms();
            cacheRepository.SaveGameData(gameData);
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == 404) // not found - game probably removed
            {
                cacheRepository.ForgetGameEntry(id);
            }
        }

        OnRefreshGame?.Invoke(id);
    }
}
