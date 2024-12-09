using System;
using System.Collections.Generic;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;

namespace Laminar.Avalonia.ViewModels.Design;

public class MockDataManager : IPersistentDataManager
{
    private static readonly string StaticPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Project Laminar", "Mock Data");
    private readonly MockDataStore _dataStore = new(StaticPath);
    
    public IPersistentDataStore GetDataStore(DataStoreKey dataStoreKey)
    {
        return _dataStore;
    }

    public string Path => StaticPath;

    private class MockDataStore(string filePath) : IPersistentDataStore
    {
        private readonly Dictionary<string, object> _dataStore = []; 
        
        public string FilePath { get; } = filePath;
        
        public DataReadResult<T> GetItem<T>(string key) where T : notnull
        {
            return _dataStore.TryGetValue(key, out var result) && result is T typedResult ? new DataReadResult<T>(typedResult) : default;
        }

        public DataReadResult<object> GetItem(string key, Type type)
        {
            return _dataStore.TryGetValue(key, out var result) && result.GetType() == type ? new DataReadResult<object>(result) : default;
        }

        public DataSaveResult SetItem<T>(string key, T value) where T : notnull
        {
            _dataStore[key] = value;
            return new DataSaveResult();
        }

        public DataSaveResult SetItem(string key, object value, Type type)
        {
            _dataStore[key] = value;
            return new DataSaveResult();
        }

        public IPersistentDataStore InitializeDefaultValue<T>(string key, T value) where T : notnull
        {
            _dataStore[key] = value;
            return this;
        }

        public IPersistentDataStore InitializeDefaultValue(string key, object value, Type type)
        {
            _dataStore[key] = value;
            return this;
        }
    }
}