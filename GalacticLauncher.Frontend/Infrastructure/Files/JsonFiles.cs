using System.Text.Json;
using System.IO;
using System;

namespace GalacticLauncher.Frontend.Infrastructure.Files;

public interface IJsonFiles
{
    void Save<T>(string filePath, T data) where T : class;
    T? Load<T>(string filePath) where T : class;
}

internal class JsonFiles : IJsonFiles
{
    public void Save<T>(string filePath, T data) where T : class
    {
        string dirPath = Path.GetDirectoryName(filePath)
            ?? throw new ArgumentException("Invalid file path.", nameof(filePath));

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(filePath, json);
    }

    public T? Load<T>(string filePath) where T : class
    {
        try
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch { return null; }
    }
}
