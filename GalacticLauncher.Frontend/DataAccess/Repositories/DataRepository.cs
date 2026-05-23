using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Infrastructure.Files;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace GalacticLauncher.Frontend.DataAccess.Repositories;

public interface IDataRepository
{
    IEnumerable<long> GetAll(string ckey);
    void Add(string ckey, long id);
    void Remove(string ckey, long id);
    void Clear(string ckey);
}

internal class DataRepository : IDataRepository
{
    private const string DATA_FILENAME = "launcher_data.json";

    private readonly Dictionary<string, HashSet<long>> _data = [];

    private readonly IJsonFiles _jsonFiles;

    public DataRepository(IJsonFiles jsonFiles)
    {
        _jsonFiles = jsonFiles;

        LoadFromDisk();
    }

    public IEnumerable<long> GetAll(string ckey)
    {
        return _data.TryGetValue(ckey, out var set) ? [.. set] : [];
    }

    public void Add(string ckey, long id)
    {
        if (!_data.TryGetValue(ckey, out var set))
        {
            _data[ckey] = set = [];
        }

        set.Add(id);
        SaveToDisk();
    }

    public void Remove(string ckey, long id)
    {
        if (_data.TryGetValue(ckey, out var set) && set.Remove(id))
        {
            if (set.Count == 0)
            {
                _data.Remove(ckey);
            }

            SaveToDisk();
        }
    }

    public void Clear(string ckey)
    {
        if (_data.Remove(ckey))
        {
            SaveToDisk();
        }
    }

    private void LoadFromDisk()
    {
        string filePath = Path.Combine(Utils.RootPath, DATA_FILENAME);

        _data.Clear();

        DataStorage? model;
        if ((model = _jsonFiles.Load<DataStorage>(filePath)) != null) // any errors = reset data
        {
            var keys = model.Keys;
            var values = model.Values;

            foreach (var (key, value) in keys.Zip(values))
            {
                _data[key] = [.. value];
            }
        }
    }

    private void SaveToDisk()
    {
        string filePath = Path.Combine(Utils.RootPath, DATA_FILENAME);

        DataStorage model = new()
        {
            Keys = [.. _data.Keys],
            Values = [.. _data.Values.Select(hset => hset.ToImmutableArray())],
        };

        _jsonFiles.Save(filePath, model);
    }
}
