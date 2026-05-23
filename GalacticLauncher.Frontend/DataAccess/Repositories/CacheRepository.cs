using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalacticLauncher.Core;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Infrastructure.Files;

namespace GalacticLauncher.Frontend.DataAccess.Repositories;

public interface ICacheRepository
{
    IEnumerable<Game> GetAllGames();
    Game? GetGame(long id);
    GameData? GetGameData(long id);
    void SaveAllGames(IEnumerable<Game> games);
    void SaveGameData(GameData gameData);
    void ForgetGameEntry(long id);
}

internal class CacheRepository : ICacheRepository
{
    private const string CACHE_FILENAME = "launcher_cache.json";

    private readonly Dictionary<long, Game> _gameCache = [];
    private readonly Dictionary<long, GameData> _gameDataCache = [];

    private readonly IJsonFiles _jsonFiles;

    public CacheRepository(IJsonFiles jsonFiles)
    {
        _jsonFiles = jsonFiles;

        LoadFromDisk();
    }

    public IEnumerable<Game> GetAllGames()
    {
        return [.. _gameCache.Values];
    }

    public Game? GetGame(long id)
    {
        return _gameCache.TryGetValue(id, out var game) ? game : null;
    }

    public GameData? GetGameData(long id)
    {
        return _gameDataCache.TryGetValue(id, out var data) ? data : null;
    }

    public void SaveAllGames(IEnumerable<Game> games)
    {
        _gameCache.Clear();

        foreach (var game in games)
        {
            _gameCache[game.Id] = game;
        }

        foreach (long id in _gameDataCache.Keys.ToList())
        {
            if (!_gameCache.ContainsKey(id))
            {
                _gameCache.Remove(id);
                _gameDataCache.Remove(id);
            }
        }

        SaveToDisk();
    }

    public void SaveGameData(GameData gameData)
    {
        _gameDataCache[gameData.Id] = gameData;

        SaveToDisk();
    }

    public void ForgetGameEntry(long id)
    {
        _gameCache.Remove(id);
        _gameDataCache.Remove(id);

        SaveToDisk();
    }

    private void LoadFromDisk()
    {
        string filePath = Path.Combine(Utils.RootPath, CACHE_FILENAME);

        _gameCache.Clear();
        _gameDataCache.Clear();

        CacheStorage? model;
        if ((model = _jsonFiles.Load<CacheStorage>(filePath)) != null) // any errors = reset cache
        {
            model.GameCache.ToList()
                .ForEach(game => _gameCache[game.Id] = game);

            model.GameDataCache.ToList()
                .ForEach(gameData => _gameDataCache[gameData.Id] = gameData);
        }
    }

    private void SaveToDisk()
    {
        string filePath = Path.Combine(Utils.RootPath, CACHE_FILENAME);

        CacheStorage model = new()
        {
            GameCache = [.. _gameCache.Values],
            GameDataCache = [.. _gameDataCache.Values],
        };

        _jsonFiles.Save(filePath, model);
    }
}
