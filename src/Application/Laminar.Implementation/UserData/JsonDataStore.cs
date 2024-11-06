using System.IO;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;
using Laminar.PluginFramework.Serialization;
using Newtonsoft.Json;

namespace Laminar.Implementation.UserData;

public class JsonDataStore(ISerializer serializer, string path) : IPersistentDataStore
{
    private readonly ISerializer _serializer = serializer;
    private readonly string _path = path;
    
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
    };
    
    public DataReadResult<T> GetItem<T>(string key)
    {
        var filePath = Path.Combine(_path, key);
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
        if (!Directory.Exists(Path.GetDirectoryName(_path)) && Path.GetDirectoryName(_path) is { } saveDirectory)
        {
            Directory.CreateDirectory(saveDirectory);
        }

        if (_serializer.TrySerialize(value) is not { } serialzied)
        {
            return new DataSaveResult(DataIoStatus.SerializerNotRegistered);
        }

        var json = JsonConvert.SerializeObject(serialzied, Formatting.Indented, JsonSettings);

        var savePath = Path.Combine(_path, key);
        using var stream = File.CreateText(savePath);
        stream.Write(json);
        return new DataSaveResult(DataIoStatus.Success);
    }
}