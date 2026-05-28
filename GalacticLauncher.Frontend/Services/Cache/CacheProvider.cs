using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalacticLauncher.Frontend.Services.Cache;

public interface ICacheProvider
{
    IEnumerable<long> GetAllGameIds();
    IEnumerable<Tag> GetAllTags();
    IEnumerable<long> GetGameTagIds(long id);

    GameDisplay GetDisplayOf(long id);
    VersionDisplay[] GetVersionDisplaysOf(long id);
}

internal class CacheProvider(
    ICacheRepository cacheRepository) : ICacheProvider
{
    public IEnumerable<long> GetAllGameIds()
    {
        return [.. cacheRepository.GetAllGames()
            .Select(game => game.Id)];
    }

    public IEnumerable<Tag> GetAllTags()
    {
        return cacheRepository.GetAllTags();
    }

    public IEnumerable<long> GetGameTagIds(long gameId)
    {
        GameData? gameData = cacheRepository.GetGameData(gameId);
        if (gameData?.Tags == null) return [];

        var allTagIds = cacheRepository.GetAllTags()
            .Select(t => t.Id);

        var gameTagIds = gameData.Tags
            .Select(t => t.Id);

        return gameTagIds.Intersect(allTagIds);
    }

    public GameDisplay GetDisplayOf(long id)
    {
        Game? game = cacheRepository.GetGame(id);
        GameData? gameData = cacheRepository.GetGameData(id);

        if (game == null && gameData != null)
        {
            throw new InvalidOperationException(
                $"Inconsistent {nameof(Game)} / {nameof(GameData)} existence at id {id}.");
        }

        return gameData?.ToDisplay()
            ?? game?.ToDisplay()
            ?? FallbackDisplay(id);

        static GameDisplay FallbackDisplay(long id) => new()
        {
            Id = id,
            Title = $"Unknown Game {id}",
            Description = "",
            IconUrl = null,
        };
    }

    public VersionDisplay[] GetVersionDisplaysOf(long id)
    {
        GameData? gameData = cacheRepository.GetGameData(id);
        return gameData?.Versions
            .Select(v => v.ToDisplay()).ToArray()
            ?? [];
    }
}
