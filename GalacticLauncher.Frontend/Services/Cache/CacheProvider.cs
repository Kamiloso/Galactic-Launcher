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
    IEnumerable<long> AllGameIds();
    IEnumerable<long> GetTagsForGame(long gameId);

    IEnumerable<Tag> GetAllTags();

    GameDisplay GetDisplayOf(long id);
    VersionDisplay[] GetVersionDisplaysOf(long id);
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
                $"Inconsistent {nameof(Game)} / {nameof(GameData)} existence at id {id}.");
        }

        return game?.ToDisplay(gameData)
            ?? new GameDisplay()
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

    public IEnumerable<Tag> GetAllTags()
    {
        return cacheRepository.GetAllTags();
    }

    public IEnumerable<long> GetTagsForGame(long gameId)
    {
        GameData? gameData = cacheRepository.GetGameData(gameId);
        if (gameData?.Tags == null) return Enumerable.Empty<long>();

        var allSystemTagIds = cacheRepository.GetAllTags().Select(t => t.Id);
        var gameTagIds = GetGameTagsIds(gameData);

        return gameTagIds.Intersect(allSystemTagIds);
    }

    private IEnumerable<long> GetGameTagsIds(GameData gameData)
    {
        List<long> ids = [];
        foreach(var tag in gameData.Tags)
        {
            ids.Add(tag.Id);
        }
        return ids;
    }
}
