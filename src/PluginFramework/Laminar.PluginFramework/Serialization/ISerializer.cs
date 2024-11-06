namespace Laminar.PluginFramework.Serialization;

public interface ISerializer
{
    public object? TrySerializeObject(object toSerialize);

    public object? TryDeserializeObject(object serialized, Type? requestedType = null, object? deserializationContext = null);
    
    public void RegisterSerializer<T>(ITypeSerializer<T> serializer);

    public ISerialized<T>? TrySerialize<T>(T serializable);

    public T? TryDeserialize<T>(ISerialized<T> serialized, object deserializationContext);
}
