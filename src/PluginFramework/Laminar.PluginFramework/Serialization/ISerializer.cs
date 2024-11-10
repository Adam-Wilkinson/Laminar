namespace Laminar.PluginFramework.Serialization;

public interface ISerializer
{
    public object TrySerializeObject(object toSerialize);

    public object TryDeserializeObject(object serialized, Type requestedType, object? context = null);
    
    public void RegisterSerializer<T>(ITypeSerializer<T> serializer);
}

public static class SerializerExtensions
{
    public static T? TryDeserialize<T>(this ISerializer serializer, object serialized, object? deserializationContext = null)
        => serializer.TryDeserializeObject(serialized, typeof(T), deserializationContext) is T typed ? typed : default;
}