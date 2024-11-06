using System;
using System.Collections.Generic;
using System.Linq;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData;

public class Serializer : ISerializer
{
    private readonly Dictionary<Type, object> _serializers = new();

    public Serializer(IServiceProvider serviceProvider)
    {
        foreach (var type in typeof(Serializer).Assembly.GetTypes())
        {
            if (type.GetInterfaces().FirstOrDefault(x =>
                    x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ITypeSerializer<>)) is { } serializerType 
                && serviceProvider.GetService(type) is { } serializer
                && serializerType.GetGenericArguments().FirstOrDefault() is { } serializableType)
            {
                _serializers.Add(serializableType, serializer);
            }

            if (type.IsPrimitive)
            {
                _serializers.Add(type, typeof(PrimitiveSerializer<>).MakeGenericType(type));
            }
        }
    }

    public ISerialized<T>? TrySerialize<T>(T serializable)
    {
        if (!(_serializers.TryGetValue(typeof(T), out var serializer) && serializer is ITypeSerializer<T> objectSerializer))
        {
            return null;
        }

        return objectSerializer.Serialize(serializable, this);
    }

    public T? TryDeserialize<T>(ISerialized<T> serialized, object deserializationContext)
    {
        if (!(_serializers.TryGetValue(typeof(T), out var serializer) && serializer is ITypeSerializer<T> objectSerializer))
        {
            return default;
        }

        return objectSerializer.DeSerialize(serialized, this, deserializationContext);
    }

    public void RegisterSerializer<T>(ITypeSerializer<T> serializer)
    {
        _serializers.Add(typeof(T), serializer);
    }

    public object TrySerializeObject(object toSerialize)
    {
        if (_serializers.TryGetValue(toSerialize.GetType(), out var serializer))
        {
            return typeof(ITypeSerializer<>).MakeGenericType(toSerialize.GetType()).GetMethod(nameof(ITypeSerializer<object>.Serialize))!.Invoke(serializer, new object[] { toSerialize, this })!;
        }

        return toSerialize;
    }

    public object TryDeserializeObject(object serialized, Type? requestedType, object? deserializationContext)
    {
        if (requestedType is not null)
        {
            if (requestedType.IsEnum)
            {
                return Enum.ToObject(requestedType, serialized);
            }
        }

        var deserializedType = serialized.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISerialized<>))?.GetGenericArguments()[0];
        if (deserializedType is not null && _serializers.TryGetValue(deserializedType, out var serializer))
        {
            return typeof(ITypeSerializer<>).MakeGenericType(deserializedType).GetMethod(nameof(ITypeSerializer<object>.DeSerialize))!.Invoke(serializer, new object[] { serialized, this, deserializationContext! })!;
        }


        if (requestedType is not null)
        {
        }

        return serialized;
    }

    private class PrimitiveSerializer<T> : ITypeSerializer<T>
    {
        public ISerialized<T> Serialize(T toSerialize, ISerializer serializer)
        {
            return new PrimitiveSerialized<T>(toSerialize);
        }

        public T DeSerialize(ISerialized<T> serialized, ISerializer serializer, object deserializationContext)
        {
            return (serialized as PrimitiveSerialized<T>)!.Value;
        }
    }

    private record PrimitiveSerialized<T>(T Value) : ISerialized<T>;
}
