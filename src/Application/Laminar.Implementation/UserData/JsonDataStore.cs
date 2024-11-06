using System.IO;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;
using Laminar.PluginFramework.Serialization;
using Newtonsoft.Json;

namespace Laminar.Implementation.UserData;

public class JsonDataStore : IPersistentDataStore
{
    private readonly ISerializer _serializer;

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
    };

    public JsonDataStore(ISerializer serializer, string path)
    {
        _serializer = serializer;
        Path = path;

        if (!Directory.Exists(Path))
        {
            Directory.CreateDirectory(Path);
        }
    }
    
    public string Path { get; }

    public DataReadResult<T> GetItem<T>(string key)
    {
        var filePath = System.IO.Path.Combine(Path, key);
        if (!File.Exists(filePath))
        {
            return new DataReadResult<T>(default, DataIoStatus.FileDoesNotExist);
        }

        var json = File.ReadAllText(filePath);

        if (JsonConvert.DeserializeObject<ISerialized<T>>(json,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }) is not { } fromJson)
        {
            return new DataReadResult<T>(default, DataIoStatus.UnknownError);
        }

        if (_serializer.TryDeserialize(fromJson, null) is not { } result)
        {
            return new DataReadResult<T>(default, DataIoStatus.SerializerNotRegistered);
        }

        return new DataReadResult<T>(result, DataIoStatus.Success);
    }

    public DataSaveResult SetItem<T>(string key, T value)
    {
        if (!Directory.Exists(System.IO.Path.GetDirectoryName(Path)) && System.IO.Path.GetDirectoryName(Path) is { } saveDirectory)
        {
            Directory.CreateDirectory(saveDirectory);
        }

        if (_serializer.TrySerialize(value) is not { } serialzied)
        {
            return new DataSaveResult(DataIoStatus.SerializerNotRegistered);
        }

        var json = JsonConvert.SerializeObject(serialzied, Formatting.Indented, JsonSettings);

        var savePath = System.IO.Path.Combine(Path, key);
        using var stream = File.CreateText(savePath);
        stream.Write(json);
        return new DataSaveResult(DataIoStatus.Success);
    }
}