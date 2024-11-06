using System;
using System.Collections.Generic;
using System.IO;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;
using Laminar.Domain.Exceptions;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData;

public class PersistentDataManager(ISerializer serializer) : IPersistentDataManager
{
    private static readonly string StaticPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Project Laminar"); 
    private readonly ISerializer _serializer = serializer;
    private readonly Dictionary<(string path, PersistentDataType dataType), IPersistentDataStore> _dataStores = new();
    
    static PersistentDataManager()
    {
        if (!Directory.Exists(StaticPath))
        {
            Directory.CreateDirectory(StaticPath);
        }
    }

    public string Path => StaticPath;

    public IPersistentDataStore GetDataStore(string dataStoreName, PersistentDataType dataType)
    {
        if (_dataStores.TryGetValue((dataStoreName, dataType), out var dataStore))
        {
            return dataStore;
        }

        var newDataStore = dataType switch
        {
            PersistentDataType.Json => new JsonDataStore(_serializer, System.IO.Path.Combine(Path, dataStoreName)),
            _ => throw new UnknownDataTypeException(dataType)
        };
        _dataStores[(dataStoreName, dataType)] = newDataStore;
        return newDataStore;
    }
}