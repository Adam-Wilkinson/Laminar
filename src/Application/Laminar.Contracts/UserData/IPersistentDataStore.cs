using Laminar.Domain.DataManagement;

namespace Laminar.Contracts.UserData;

public interface IPersistentDataStore
{
    public string FilePath { get; }

    public DataReadResult<T> GetItem<T>(string key)
        where T : notnull;

    public DataReadResult<object> GetItem(string key, Type type);
    
    public DataSaveResult SetItem<T>(string key, T value)
        where T : notnull;
    
    public DataSaveResult SetItem(string key, object value, Type type);

    public IPersistentDataStore InitializeDefaultValue<T>(string key, T value, object? deserializationContext = null)
        where T : notnull;
    
    public IPersistentDataStore InitializeDefaultValue(string key, object value, Type type, object? deserializationContext = null);
}