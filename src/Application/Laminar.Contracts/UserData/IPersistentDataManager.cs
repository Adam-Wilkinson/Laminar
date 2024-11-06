using Laminar.Domain.DataManagement;

namespace Laminar.Contracts.UserData;

public interface IPersistentDataManager
{
    public IPersistentDataStore GetDataStore(string dataStoreName, PersistentDataType dataType);

    public string Path { get; }
}