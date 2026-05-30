using GalacticLauncher.Core;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Tools.Files;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GalacticLauncher.Frontend.Repositories;

public interface ICacheRepository
{
    Game? GetGame(long id);
    IEnumerable<long> GetAllGames();
    void UpdateGame(Game game);
    void UpdateMoreGames(IEnumerable<Game> games, bool clearOther);

    Tag? GetTag(long id);
    IEnumerable<long> GetAllTags();
    void OverwriteAllTags(IEnumerable<Tag> tags);
}

internal class CacheRepository : ICacheRepository
{
    private const string CACHE_FILENAME = "launcher_cache.json";

    private readonly Dictionary<long, Game> _gameCache = [];
    private readonly Dictionary<long, Tag> _tagsCache = [];

    private readonly IJsonFiles _jsonFiles;

    public CacheRepository(IJsonFiles jsonFiles)
    {
        _jsonFiles = jsonFiles;

        LoadFromDisk();
    }

    public Game? GetGame(long id) =>
        _gameCache.TryGetValue(id, out var game) ? game : null;

    public IEnumerable<long> GetAllGames() =>
        [.. _gameCache.Keys];

    public void UpdateGame(Game game)
    {
        UpdateMoreGames([game], clearOther: false);
    }

    public void UpdateMoreGames(IEnumerable<Game> games, bool clearOther)
    {
        foreach (var game in games)
        {
            Game? oldGame = GetGame(game.Id);

            _gameCache[game.Id] = oldGame is GameData oldGameData
                ? oldGameData.Inject(game)
                : game;
        }

        if (clearOther)
        {
            List<long> existing = [.. _gameCache.Keys];

            foreach (long id in existing)
            {
                if (!games.Any(g => g.Id == id))
                {
                    _gameCache.Remove(id);
                }
            }
        }

        SaveToDisk();
    }

    public Tag? GetTag(long id) =>
        _tagsCache.TryGetValue(id, out var tag) ? tag : null;

    public IEnumerable<long> GetAllTags() =>
        [.. _tagsCache.Keys];

    public void OverwriteAllTags(IEnumerable<Tag> tags)
    {
        _tagsCache.Clear();

        foreach (var tag in tags)
        {
            _tagsCache[tag.Id] = tag;
        }

        SaveToDisk();
    }

    #region Disk Storage

    private record CacheStorage
    {
        public required Game[]? GameCache { get; init; }
        public required GameData[]? GameDataCache { get; init; }
        public required Tag[]? TagsCache { get; init; }
    }

    private void LoadFromDisk()
    {
        string filePath = Path.Combine(Utils.RootPath, CACHE_FILENAME);

        _gameCache.Clear();
        _tagsCache.Clear();

        if (Utils.IsDevelopment && Utils.DevelopmentIgnoreCache)
            return; // skip loading cache if needed (only in development)

        CacheStorage? model;
        if ((model = _jsonFiles.Load<CacheStorage>(filePath)) != null) // any errors = reset cache
        {
            model.GameCache?.ToList()
                .ForEach(game => _gameCache[game.Id] = game);

            model.GameDataCache?.ToList()
                .ForEach(gameData => _gameCache[gameData.Id] = gameData);

            model.TagsCache?.ToList()
                .ForEach(tag => _tagsCache[tag.Id] = tag);
        }
    }

    private void SaveToDisk()
    {
        string filePath = Path.Combine(Utils.RootPath, CACHE_FILENAME);

        CacheStorage model = new()
        {
            GameCache = [.. _gameCache.Values.Where(g => g is not GameData)],
            GameDataCache = [.. _gameCache.Values.Where(g => g is GameData).Cast<GameData>()],
            TagsCache = [.. _tagsCache.Values]
        };

        _jsonFiles.Save(filePath, model);
    }

    #endregion
}
