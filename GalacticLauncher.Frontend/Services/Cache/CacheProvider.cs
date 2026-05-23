using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.DataAccess.Repositories;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalacticLauncher.Frontend.Services.Cache;

public interface ICacheProvider
{
    public IEnumerable<long> AllGameIds();
    public GameDisplay GetDisplayOf(long id);
}

internal class CacheProvider(
    ICacheRepository cacheRepository) : ICacheProvider
{
    public IEnumerable<long> AllGameIds()
    {
        return [.. cacheRepository.GetAllGames()
            .Select(game => game.Id)];
    }

    public GameDisplay GetDisplayOf(long id)
    {
        Game? game = cacheRepository.GetGame(id);
        GameData? gameData = cacheRepository.GetGameData(id);

        if (game == null && gameData != null)
        {
            throw new InvalidOperationException(
                $"{nameof(GameData)} exists where {nameof(Game)} doesn't exist.");
        }

        return game?.ToDisplay(gameData)
            ?? new GameDisplay
            {
                Title = $"Unknown Game (id: {id})",
                Description = "...",
                IconUrl = null,
            };
    }
}
