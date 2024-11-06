using System.IO;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;
using Laminar.PluginFramework.Serialization;
using Newtonsoft.Json;

namespace Laminar.Implementation.UserData;

public class PersistentDataStore : IPersistentDataStore
{
    private readonly ISerializer _serializer;
    private readonly IFileSaver _saver;
    
    public PersistentDataStore(ISerializer serializer, IFileSaver saver, string path)
    {
        _serializer = serializer;
        _saver = saver;
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

        var readResult = _saver.Load<ISerialized<T>>(filePath);
        if (readResult.Status != DataIoStatus.Success || readResult.Result is not { } readObject)
        {
            return new DataReadResult<T>(default, readResult.Status);
        }
        
        if (_serializer.TryDeserialize(readObject) is not { } result)
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

        if (_serializer.TrySerialize(value) is not { } serialized)
        {
            return new DataSaveResult(DataIoStatus.SerializerNotRegistered);
        }

        return _saver.Save(System.IO.Path.Combine(Path, key), serialized);
    }
}