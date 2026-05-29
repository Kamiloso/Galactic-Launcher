using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace GalacticLauncher.Frontend.Services.Data;

public interface ICacheProvider
{
    IEnumerable<long> GetAllGameIds();
    IEnumerable<Tag> GetAllTags();
    IEnumerable<long> GetGameTagIds(long id);

    Game? GetGameOf(long id);
    GameData? GetGameDataOf(long id);
    Version[] GetVersionsOf(long id);
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

    public Game? GetGameOf(long id)
    {
        return cacheRepository.GetGameData(id)
            ?? cacheRepository.GetGame(id);
    }

    public GameData? GetGameDataOf(long id)
    {
        return cacheRepository.GetGameData(id);
    }

    public Version[] GetVersionsOf(long id)
    {
        return cacheRepository.GetGameData(id)
            ?.Versions.ToArray() ?? [];
    }
}
