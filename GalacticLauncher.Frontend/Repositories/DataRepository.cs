using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Tools.Files;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace GalacticLauncher.Frontend.Repositories;

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

    private readonly Dictionary<string, List<long>> _data = [];

    private readonly IJsonFiles _jsonFiles;
 
    public DataRepository(IJsonFiles jsonFiles)
    {
        _jsonFiles = jsonFiles;

        LoadFromDisk();
    }

    public IEnumerable<long> GetAll(string ckey)
    {
        return _data.TryGetValue(ckey, out var list) ? [.. list] : [];
    }

    public void Add(string ckey, long id)
    {
        if (!_data.TryGetValue(ckey, out var list))
        {
            _data[ckey] = list = [];
        }
        if (!list.Contains(id))
        {
            list.Add(id);
            SaveToDisk();
        }
    }

    public void Remove(string ckey, long id)
    {
        if (_data.TryGetValue(ckey, out var list) && list.Remove(id))
        {
            if (list.Count == 0)
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
            Values = [.. _data.Values.Select(list => list.ToImmutableArray())],
        };

        _jsonFiles.Save(filePath, model);
    }
}
