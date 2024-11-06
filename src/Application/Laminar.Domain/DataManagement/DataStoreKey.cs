namespace Laminar.Domain.DataManagement;

public record DataStoreKey(string Name, PersistentDataType DataType)
{
    public static DataStoreKey PersistentData { get; } = new DataStoreKey("PersistentData", PersistentDataType.Json);
}

public enum PersistentDataType
{
    Json = 1,
}