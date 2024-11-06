using Laminar.Domain.DataManagement;

namespace Laminar.Contracts.UserData;

public interface IPersistentDataStore
{
    public string Path { get; }

    public DataReadResult<T> GetItem<T>(string key);
    
    public DataSaveResult SetItem<T>(string key, T value);
}