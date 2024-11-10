using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using Laminar.PluginFramework.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Implementation.UserData;

public class Serializer : ISerializer
{
    private readonly Dictionary<Type, object> _serializers = new();

    public Serializer(IServiceProvider serviceProvider)
    {
        var assembly = typeof(Serializer).Assembly;
        foreach (var type in assembly.GetTypes())
        {
            if (type.GetInterfaces().FirstOrDefault(x =>
                    x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ITypeSerializer<>)) is { } serializerType 
                && serviceProvider.GetService(type) is { } serializer
                && serializerType.GetGenericArguments().FirstOrDefault() is { } serializableType)
            {
                _serializers.Add(serializableType, serializer);
            }
        }
    }

    public void RegisterSerializer<T>(ITypeSerializer<T> serializer)
    {
        _serializers.Add(typeof(T), serializer);
    }

    public object? TrySerializeObject(object toSerialize)
    {
        if (toSerialize.GetType().IsPrimitive)
        {
            return toSerialize;
        }
        
        if (_serializers.TryGetValue(toSerialize.GetType(), out var typeSerializer))
        {
            return CallGenericSerialize(typeSerializer, toSerialize);
        }

        if (toSerialize.GetType().GetInterfaces().Contains(typeof(IEnumerable))
            && Activator.CreateInstance(typeof(EnumerableSerializer<>).MakeGenericType(toSerialize.GetType()), this) is { } enumerableSerializer)
        {
            _serializers.Add(toSerialize.GetType(), enumerableSerializer);
            return CallGenericSerialize(enumerableSerializer, toSerialize);            
        }

        return toSerialize;
    }

    public object? TryDeserializeObject(object serialized, Type requestedType, object? context)
    {
        if (serialized.GetType().IsPrimitive)
        {
            return serialized;
        }

        if (_serializers.TryGetValue(requestedType, out var typeSerializer))
        {
            return CallGenericDeSerialize(typeSerializer, serialized, requestedType, context);
        }

        if (serialized.GetType().GetInterfaces().Contains(typeof(IEnumerable))
            && Activator.CreateInstance(typeof(EnumerableSerializer<>).MakeGenericType(requestedType), this) is
                { } enumerableSerializer)
        {
            _serializers.Add(requestedType, enumerableSerializer);
            var enumerableDeserialized =
                CallGenericDeSerialize(enumerableSerializer, serialized, requestedType, context);
            return typeof(Serializer).GetMethod(nameof(EnumerableToCollection), requestedType.GetGenericArguments())!
                .Invoke(this, [enumerableDeserialized, requestedType, context]);
        }

        return serialized;
    }

    private object CallGenericSerialize(object typeSerializer, object toSerialize)
    {
        return typeof(ITypeSerializer<>).MakeGenericType(toSerialize.GetType())
            .GetMethod(nameof(ITypeSerializer<object>.Serialize))!
            .Invoke(typeSerializer, [toSerialize, this])!;
    }

    private object CallGenericDeSerialize(object typeSerializer, object serialized, Type deserializedType, object? deserializationContext)
    {
        return typeof(ITypeSerializer<>).MakeGenericType(deserializedType)
            .GetMethod(nameof(ITypeSerializer<object>.DeSerialize))!
            .Invoke(typeSerializer, [serialized, this, deserializationContext])!;
    }

    private static object EnumerableToCollection<T>(IEnumerable<T> obj, Type requestedType)
    {
        if (requestedType.IsArray) return obj.ToArray();
        if (requestedType.IsAssignableFrom(typeof(IList<T>))) return obj.ToList();
        return obj;
    }
    
    private class EnumerableSerializer<T>(ISerializer serializer) : ITypeSerializer<IEnumerable<T>>
    {
        private readonly ISerializer _serializer = serializer; 
        
        public ISerialized<IEnumerable<T>> Serialize(IEnumerable<T> toSerialize, ISerializer serializer)
        {
            return new SerializedEnumerable<T>(toSerialize.Select(x => _serializer.TrySerializeObject(x))!);
        }

        public IEnumerable<T> DeSerialize(ISerialized<IEnumerable<T>> serialized, ISerializer serializer, object? deserializationContext)
        {
            if (serialized is not SerializedEnumerable<T> enumerable)
            {
                yield break;
            }

            foreach (var serializedElement in enumerable.Serialized)
            {
                yield return _serializer.TryDeserialize<T>(serializedElement, deserializationContext)!;
            }
        }
    }
    
    private record SerializedEnumerable<T>(IEnumerable<object> Serialized) : ISerialized<IEnumerable<T>>;
}
