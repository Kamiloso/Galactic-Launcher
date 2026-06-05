using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Tools.Files;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GalacticLauncher.Frontend.Repositories;

public interface IMemoryRepository
{
    string this[string mkey] { get; set; }
}

internal class MemoryRepository : IMemoryRepository
{
    private const string DATA_FILENAME = "launcher_memory.json";

    private readonly Dictionary<string, string> _memdict = [];

    private readonly IJsonFiles _jsonFiles;
 
    public MemoryRepository(IJsonFiles jsonFiles)
    {
        _jsonFiles = jsonFiles;

        LoadFromDisk();
    }

    public string this[string mkey]
    {
        get => _memdict.TryGetValue(mkey, out var value) ? value : "";
        set
        {
            _memdict[mkey] = value;

            SaveToDisk();
        }
    }

    #region Disk Storage

    private record MemoryStorage
    {
        public required Dictionary<string, string>? Dictionary { get; init; }
    }

    private void LoadFromDisk()
    {
        string filePath = Path.Combine(Utils.RootPath, DATA_FILENAME);

        _memdict.Clear();

        MemoryStorage? model;
        if ((model = _jsonFiles.Load<MemoryStorage>(filePath)) != null) // any errors = reset data
        {
            _memdict.Clear();

            foreach (var (key, value) in model.Dictionary ?? [])
            {
                _memdict[key] = value;
            }
        }
    }

    private void SaveToDisk()
    {
        string filePath = Path.Combine(Utils.RootPath, DATA_FILENAME);

        MemoryStorage model = new()
        {
            Dictionary = _memdict.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value),
        };

        _jsonFiles.Save(filePath, model);
    }

    #endregion
}
