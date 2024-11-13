using System;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData.Serializers;

public class DefaultSerializer : IConditionalSerializer
{
    public Type? SerializedTypeOrNull(Type typeToSerialize) => typeToSerialize;

    public object Serialize(object toSerialize) => toSerialize;

    public object DeSerialize(object serialized, object? deserializationContext = null) => serialized;
}