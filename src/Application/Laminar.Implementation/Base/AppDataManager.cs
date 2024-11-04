using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Laminar.Contracts.Base;
using Newtonsoft.Json;

namespace Laminar.Implementation.Base;

internal class AppDataManager : IUserDataStore
{
    private static readonly string AppDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Project Laminar");
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
    };

    static AppDataManager()
    {
        if (!Directory.Exists(AppDataLocation))
        {
            Directory.CreateDirectory(AppDataLocation);
        }
    }

    public IEnumerable<T> LoadAllFromFolder<T>(string folder, string fileType)
    {
        if (!Directory.Exists(Path.Combine(AppDataLocation, folder))) yield break;
        
        foreach (var dir in Directory.EnumerateFiles(Path.Combine(AppDataLocation, folder), $"*.{fileType}"))
        {
            yield return Load<T>(dir);
        }
    }

    public T Load<T>(string fileName)
    {
        if (!File.Exists(Path.Combine(AppDataLocation, fileName)))
        {
            throw new FileNotFoundException($"The file {fileName} does not exist.");
        }

        var json = File.ReadAllText(Path.Combine(AppDataLocation, fileName));

        if (JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }) is not { } deserializeObject)
        {
            throw new Exception($"The file {fileName} has not been deserialized.");
        }
        
        return deserializeObject;
    }

    public void Save(string fileName, object toSave)
    {
        var savePath = Path.Combine(AppDataLocation, fileName);
        if (!Directory.Exists(Path.GetDirectoryName(savePath)) && Path.GetDirectoryName(savePath) is { } saveDirectory)
        {
            Directory.CreateDirectory(saveDirectory);
        }

        var json = JsonConvert.SerializeObject(toSave, Formatting.Indented, JsonSettings);

        using var stream = File.CreateText(Path.Combine(AppDataLocation, fileName));
        stream.Write(json);
    }
    
    public bool TryLoad<T>(string fileName, out T? loaded)
    {
        if (!File.Exists(Path.Combine(AppDataLocation, fileName)))
        {
            loaded = default;
            return false;
        }

        var json = File.ReadAllText(Path.Combine(AppDataLocation, fileName));
        loaded = JsonConvert.DeserializeObject<T>(json, JsonSettings);
        return true;
    }
}
