using System;
using System.Collections.Generic;
using System.IO;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData;

public class PersistentDataStore : IPersistentDataStore
{
    private readonly ISerializer _serializer;
    private readonly IDataTranscoder _dataTranscoder;

    
    private readonly Dictionary<string, PersistentValue> _serializedDataCache;
    
    public PersistentDataStore(ISerializer serializer, IDataTranscoder dataTranscoder, string dataPath)
    {
        _serializer = serializer;
        _dataTranscoder = dataTranscoder;
        FilePath = dataPath + _dataTranscoder.FileExtension;

        if (!(Directory.GetParent(FilePath) is { } dir && dir.Exists))
        {
            Directory.CreateDirectory(Directory.GetParent(FilePath)!.FullName);
        }
        
        if (!File.Exists(FilePath))
        {
            File.Create(FilePath).Close();
        }

        var data = File.ReadAllText(FilePath);
        _serializedDataCache = _dataTranscoder.Decode<Dictionary<string, PersistentValue>>(data) ?? [];
    }
    
    public string FilePath { get; }

    public DataReadResult<T> GetItem<T>(string key)
    {
        if (!_serializedDataCache.TryGetValue(key, out var value))
        {
            return new DataReadResult<T>(default, DataIoStatus.DataNotFound);
        }
        
        if (_serializer.TryDeserialize<T>(value) is not { } result)
        {
            return new DataReadResult<T>(default, DataIoStatus.SerializerNotRegistered);
        }

        return new DataReadResult<T>(result);
    }

    public DataSaveResult SetItem<T>(string key, T value)
    {
        if (value is null || _serializer.TrySerializeObject(value) is not { } serialized)
        {
            return new DataSaveResult(DataIoStatus.SerializerNotRegistered);
        }

        if (!_serializedDataCache.TryGetValue(key, out var persistentValue))
        {
            return new DataSaveResult(DataIoStatus.DataNotFound);
        }
        
        persistentValue.Value = serialized;
        return SaveCache();
    }

    public DataSaveResult ResetToDefault(string key)
    {
        if (!_serializedDataCache.TryGetValue(key, out var persistentValue))
        {
            return new DataSaveResult(DataIoStatus.DataNotFound);
        }
        
        persistentValue.ResetToDefault();
        return new DataSaveResult();
    }
    
    public IPersistentDataStore InitializeDefaultValue<T>(string key, T value)
        where T : notnull
    {
        _serializedDataCache[key] = new PersistentValue(_serializer, value);
        return this;
    }

    private DataSaveResult SaveCache()
    {
        try
        {
            var result = _dataTranscoder.Encode(_serializedDataCache);
            using var stream = File.CreateText(FilePath + _dataTranscoder.FileExtension);
            stream.Write(result);
            return new DataSaveResult(DataIoStatus.Success);
        }
        catch (Exception ex)
        {
            return new DataSaveResult(DataIoStatus.UnknownError, ex);
        }
    }
}