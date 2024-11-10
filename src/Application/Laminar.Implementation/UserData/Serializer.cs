using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using Laminar.PluginFramework.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
        if (toSerialize.GetType().IsPrimitive || toSerialize is string)
        {
            return toSerialize;
        }
        
        if (_serializers.TryGetValue(toSerialize.GetType(), out var typeSerializer))
        {
            return CallGenericSerialize(typeSerializer, toSerialize);
        }

        if (toSerialize.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)) is { } enumerableType
            && Activator.CreateInstance(typeof(EnumerableSerializer<,>).MakeGenericType(enumerableType.GenericTypeArguments[0], toSerialize.GetType()), this) is { } enumerableSerializer)
        {
            _serializers.Add(toSerialize.GetType(), enumerableSerializer);
            return CallGenericSerialize(enumerableSerializer, toSerialize);            
        }

        return toSerialize;
    }

    public object? TryDeserializeObject(object serialized, Type requestedType, object? context)
    {
        if (serialized.GetType().IsPrimitive || serialized is string)
        {
            return serialized;
        }

        if (_serializers.TryGetValue(requestedType, out var typeSerializer))
        {
            return CallGenericDeSerialize(typeSerializer, serialized, requestedType, context);
        }

        if (serialized.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)) is { } enumerableType
            && Activator.CreateInstance(typeof(EnumerableSerializer<,>).MakeGenericType(enumerableType.GenericTypeArguments[0], requestedType), this) is
                { } enumerableSerializer)
        {
            _serializers.Add(requestedType, enumerableSerializer);
            return CallGenericDeSerialize(enumerableSerializer, serialized, requestedType, context);
            
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
    
    private class EnumerableSerializer<TElement, TEnumerable>(ISerializer serializer) : ITypeSerializer<TEnumerable>
        where TEnumerable : IEnumerable<TElement>
    {
        private readonly ISerializer _serializer = serializer;

        public bool Match(TEnumerable enumerable)
            => enumerable is List<TElement> or TElement[];
        
        public ISerialized<TEnumerable> Serialize(TEnumerable toSerialize, ISerializer serializer)
        {
            return new SerializedEnumerable<TEnumerable>(toSerialize.Select(x => _serializer.TrySerializeObject(x))!);
        }

        public TEnumerable DeSerialize(ISerialized<TEnumerable> serialized, ISerializer serializer, object? context)
        {
            if (serialized is not SerializedEnumerable<TElement> enumerable)
            {
                throw new ArgumentException(nameof(serialized));
            }

            IEnumerable<TElement> deserializedEnumerable = enumerable
                    .Select(x => _serializer.TryDeserialize<TElement>(x, context))
                    .Where(x => x is not null)!;

            if (typeof(TEnumerable).IsArray && deserializedEnumerable.ToArray() is TEnumerable array) return array;
            if (typeof(TEnumerable).IsAssignableFrom(typeof(IList<TElement>)) && deserializedEnumerable.ToList() is TEnumerable list) return list;
            throw new InvalidOperationException();
        }
    }

    private class SerializedEnumerable<TEnumerable>(IEnumerable<object> serialized)
        : ISerialized<TEnumerable>, IEnumerable<object>
    {
        public IEnumerator<object> GetEnumerator() => serialized.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
