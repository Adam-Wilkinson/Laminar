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
    private readonly IPersistentDataTranscoder _persistentDataTranscoder;
    
    private Dictionary<string, IPersistentDataValue> _serializedDataCache = [];
    private bool _fileIsDirty;
    
    public PersistentDataStore(ISerializer serializer, IPersistentDataTranscoder persistentDataTranscoder, string dataPath)
    {
        _serializer = serializer;
        _persistentDataTranscoder = persistentDataTranscoder;
        FilePath = dataPath + _persistentDataTranscoder.FileExtension;

        if (Directory.GetParent(FilePath) is not { Exists: true })
        {
            Directory.CreateDirectory(Directory.GetParent(FilePath)!.FullName);
        }
        
        if (!File.Exists(FilePath))
        {
            File.Create(FilePath).Close();
        }
    }
    
    public string FilePath { get; }

    public DataReadResult<T> GetItem<T>(string key)
        where T : notnull
    {
        var objectRead = GetItem(key, typeof(T));
        return new DataReadResult<T>(objectRead.Status == DataIoStatus.Success ? (T)objectRead.Result : default, objectRead.Status, objectRead.Exception);
    }

    public DataReadResult<object> GetItem(string key, Type type)
    {
        if (_fileIsDirty)
        {
            var loadResult = LoadFromFile();
            if (loadResult.Status != DataIoStatus.Success) return new DataReadResult<object>(default, loadResult.Status, loadResult.Exception);
            var syncResult = SyncToFile();
            if (syncResult.Status != DataIoStatus.Success) return new DataReadResult<object>(default, syncResult.Status, syncResult.Exception);
            _fileIsDirty = false;
        }
        
        if (!_serializedDataCache.TryGetValue(key, out var persistentData))
        {
            return new DataReadResult<object>(default, DataIoStatus.DataNotFound);
        }

        if (persistentData.ValueType != type)
        {
            return new DataReadResult<object>(default, DataIoStatus.UnknownError);
        }
        
        return new DataReadResult<object>(persistentData.Value);
    }

    public DataSaveResult SetItem<T>(string key, T value)
        where T : notnull
        => SetItem(key, value, typeof(T));

    public DataSaveResult SetItem(string key, object value, Type type)
    {
        if (!_serializedDataCache.TryGetValue(key, out var persistentValue))
        {
            return new DataSaveResult(DataIoStatus.DataNotFound);
        }
        
        persistentValue.Value = value;
        return SyncToFile();
    }

    public DataSaveResult ResetToDefault(string key)
    {
        if (!_serializedDataCache.TryGetValue(key, out var persistentValue))
        {
            return new DataSaveResult(DataIoStatus.DataNotFound);
        }
        
        persistentValue.ResetToDefault();
        return SyncToFile();
    }
    
    public IPersistentDataStore InitializeDefaultValue<T>(string key, T value)
        where T : notnull
    {
        _serializedDataCache[key] = new PersistentValue(_serializer, value);
        _fileIsDirty = true;
        return this;
    }

    public IPersistentDataStore InitializeDefaultValue(string key, object value, Type type)
    {
        _serializedDataCache[key] = new PersistentValue(_serializer, value);
        _fileIsDirty = true;
        return this;
    }

    private DataSaveResult LoadFromFile()
    {
        try
        {
            var rawFile = File.ReadAllText(FilePath);
            if (rawFile is null or "")
            {
                return new DataSaveResult();
            }
            
            if (_persistentDataTranscoder.Decode(rawFile, _serializedDataCache) is not { } decodeResult)
            {
                return new DataSaveResult(DataIoStatus.UnknownError);
            }
            _serializedDataCache = decodeResult;
            return new DataSaveResult();
        }
        catch (Exception ex)
        {
            return new DataSaveResult(DataIoStatus.UnknownError, ex);
        }
    }
    
    private DataSaveResult SyncToFile()
    {
        try
        {
            var result = _persistentDataTranscoder.Encode(_serializedDataCache);
            using var stream = File.CreateText(FilePath);
            stream.Write(result);
            return new DataSaveResult(DataIoStatus.Success);
        }
        catch (Exception ex)
        {
            return new DataSaveResult(DataIoStatus.UnknownError, ex);
        }
    }
}