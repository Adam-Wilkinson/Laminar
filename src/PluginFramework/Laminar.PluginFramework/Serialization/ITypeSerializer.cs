namespace Laminar.PluginFramework.Serialization;

public interface ITypeSerializer<T>
{
    ISerialized<T> Serialize(T toSerialize, ISerializer serializer);

    T DeSerialize(ISerialized<T> serialized, ISerializer serializer, object deserializationContext);
}
