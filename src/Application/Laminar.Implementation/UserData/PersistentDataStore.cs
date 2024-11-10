using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData;

public class PersistentDataStore : IPersistentDataStore
{
    private readonly ISerializer _serializer;
    private readonly IFileSaver _saver;

    private readonly Dictionary<string, object> _serializedDataCache;
    private readonly Dictionary<string, object> _defaultValues = [];
    
    public PersistentDataStore(ISerializer serializer, IFileSaver saver, string dataPath)
    {
        _serializer = serializer;
        _saver = saver;
        Path = dataPath;

        if (!(Directory.GetParent(Path) is { } dir && dir.Exists))
        {
            Directory.CreateDirectory(Directory.GetParent(Path)!.FullName);
        }

        _serializedDataCache = _saver.Load<Dictionary<string, object>>(Path).Result ?? [];
    }
    
    public string Path { get; }

    public DataReadResult<T> GetItem<T>(string key)
    {
        if (!_serializedDataCache.TryGetValue(key, out var value) 
            || value is not ISerialized<T> serializedData)
        {
            return new DataReadResult<T>(default, DataIoStatus.DataNotFound);
        }
        
        if (_serializer.TryDeserialize<T>(serializedData) is not { } result)
        {
            return new DataReadResult<T>(default, DataIoStatus.SerializerNotRegistered);
        }

        return new DataReadResult<T>(result, DataIoStatus.Success);
    }

    public DataSaveResult SetItem<T>(string key, T value)
    {
        if (value is null || _serializer.TrySerializeObject(value) is not { } serialized)
        {
            return new DataSaveResult(DataIoStatus.SerializerNotRegistered);
        }
        
        _serializedDataCache[key] = serialized;
        return SaveCache();
    }

    public bool HasDefaultValue(string key)
        => _defaultValues.ContainsKey(key);

    public DataSaveResult ResetToDefault(string key)
    {
        if (!_defaultValues.TryGetValue(key, out var defaultValue))
        {
            throw new KeyNotFoundException();
        }

        _serializedDataCache[key] = defaultValue;
        return SaveCache();
    }
    
    public IPersistentDataStore InitializeDefaultValue<T>(string key, T value)
    {
        if (value is null || _serializer.TrySerializeObject(value) is not { } serialized)
        {
            throw new Exception($"Cannot serialize value of type {typeof(T)}");
        }
        
        _defaultValues[key] = serialized;

        if (!_serializedDataCache.ContainsKey(key))
        {
            SetItem(key, value);
        }
        
        return this;
    }

    private DataSaveResult SaveCache()
    {
        try
        {
            return _saver.Save(Path, _serializedDataCache);
        }
        catch (Exception ex)
        {
            return new DataSaveResult(DataIoStatus.UnknownError, ex);
        }
    }
}