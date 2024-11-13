namespace Laminar.PluginFramework.Serialization;

public interface ISerializer
{
    public object SerializeObject(object toSerialize);

    public object DeserializeObject(object serialized, Type requestedType, object? context = null);
    
    public void RegisterSerializer(IConditionalSerializer serializer);
    
    public void RegisterFactory(IConditionalSerializerFactory factory);

    public Type GetSerializedType(Type typeToSerialize);
}

public static class SerializerExtensions
{
    public static T? TryDeserialize<T>(this ISerializer serializer, object serialized, object? deserializationContext = null)
        => serializer.DeserializeObject(serialized, typeof(T), deserializationContext) is T typed ? typed : default;
}