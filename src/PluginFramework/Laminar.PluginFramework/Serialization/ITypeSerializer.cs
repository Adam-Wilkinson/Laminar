namespace Laminar.PluginFramework.Serialization;

public interface ITypeSerializer<T>
{
    bool Match(T toSerialize);
    
    ISerialized<T> Serialize(T toSerialize, ISerializer serializer);

    T DeSerialize(ISerialized<T> serialized, ISerializer serializer, object? context);
}
