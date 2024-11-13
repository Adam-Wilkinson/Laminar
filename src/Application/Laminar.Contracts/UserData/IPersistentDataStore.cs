using Laminar.Domain.DataManagement;

namespace Laminar.Contracts.UserData;

public interface IPersistentDataStore
{
    public string FilePath { get; }

    public DataReadResult<T> GetItem<T>(string key)
        where T : notnull;
    
    public DataSaveResult SetItem<T>(string key, T value)
        where T : notnull;

    public IPersistentDataStore InitializeDefaultValue<T>(string key, T value)
        where T : notnull;
}