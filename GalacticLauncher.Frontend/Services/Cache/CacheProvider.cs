using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace GalacticLauncher.Frontend.Services.Cache;

public interface ICacheProvider
{
    Game? GetGameOf(long id);
    GameData? GetGameDataOf(long id);

    IEnumerable<Game> GetAllGames();
    IEnumerable<Version> GetVersionsOf(long id);
    IEnumerable<Tag> GetAllTags();
    IEnumerable<Tag> GetTagsOf(long id);
}

public class CacheProvider(
    ICacheRepository cacheRepository) : ICacheProvider
{
    public Game? GetGameOf(long id)
    {
        return cacheRepository.GetGame(id);
    }

    public GameData? GetGameDataOf(long id)
    {
        return cacheRepository.GetGame(id) as GameData;
    }

    public IEnumerable<Game> GetAllGames()
    {
        return [.. cacheRepository.GetAllGames()
            .Select(id => cacheRepository.GetGame(id)!)];
    }

    public IEnumerable<Version> GetVersionsOf(long id)
    {
        GameData? gameData = GetGameDataOf(id);
        if (gameData is null) return [];

        return [.. gameData.Versions
            .OrderByDescending(v => v.ReleaseDate)];
    }

    public IEnumerable<Tag> GetAllTags()
    {
        return [.. cacheRepository.GetAllTags()
            .Select(id => cacheRepository.GetTag(id)!)];
    }

    public IEnumerable<Tag> GetTagsOf(long id)
    {
        Game? game = GetGameOf(id);
        if (game is null) return [];

        List<Tag> allTags = [.. GetAllTags()];

        return [.. game.ExtractTagIds()
            .Select(tagId => allTags.FirstOrDefault(t => t.Id == tagId))
            .Where(tag => tag != null)
            .Select(tag => tag!)];
    }
}
