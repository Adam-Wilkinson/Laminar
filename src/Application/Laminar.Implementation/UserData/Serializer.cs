using System;
using System.Collections.Generic;
using System.Linq;
using Laminar.Implementation.UserData.Serializers;
using Laminar.PluginFramework.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Implementation.UserData;

public class Serializer(IServiceProvider serviceProvider) : ISerializer
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    
    private Dictionary<Type, IConditionalSerializer>? _typeSerializers;
    private List<IConditionalSerializer>? _conditionalSerializers;
    private List<IConditionalSerializerFactory>? _conditionalSerializerFactories;

    public void RegisterSerializer(IConditionalSerializer serializer)
    {
        EnsureSerializersInit();
        switch (serializer)
        {
            case TypeSerializer typeSerializer:
                _typeSerializers![typeSerializer.Type] = typeSerializer;
                break;
            default:
                _conditionalSerializers!.Add(serializer);
                break;
        }
    }

    public void RegisterFactory(IConditionalSerializerFactory factory)
    {
        EnsureSerializersInit();
        _conditionalSerializerFactories!.Add(factory);
    }

    public object SerializeObject(object toSerialize)
        => GetSerializer(toSerialize.GetType()).Serialize(toSerialize);

    public object DeserializeObject(object serialized, Type requestedType, object? context)
        => GetSerializer(requestedType).DeSerialize(serialized);

    public Type GetSerializedType(Type typeToSerialize)
    {
        return GetSerializer(typeToSerialize).SerializedTypeOrNull(typeToSerialize)!;
    }
    
    private IConditionalSerializer GetSerializer(Type typeToSerialize)
    {
        EnsureSerializersInit();
        
        if (_typeSerializers!.TryGetValue(typeToSerialize, out var typeSerializer))
        {
            return typeSerializer;
        }

        foreach (var factory in _conditionalSerializerFactories!)
        {
            if (factory.TryCreateSerializerFor(typeToSerialize) is { } serializer)
            {
                _typeSerializers.Add(typeToSerialize, serializer);
                return serializer;
            }
        }
        
        if (_conditionalSerializers!.FirstOrDefault(serializer => serializer.SerializedTypeOrNull(typeToSerialize) is not null) is
            { } conditionalSerializer)
        {
            _typeSerializers.Add(typeToSerialize, conditionalSerializer);
            return conditionalSerializer;
        }

        return new DefaultSerializer();
    }

    private void EnsureSerializersInit()
    {
        if (_conditionalSerializerFactories is not null && _conditionalSerializers is not null &&
            _typeSerializers is not null)
        {
            return;
        }

        _conditionalSerializerFactories = [];
        _typeSerializers = [];
        _conditionalSerializers = [];
        
        var assembly = typeof(Serializer).Assembly;
        foreach (var type in assembly.GetTypes())
        {
            if (!type.ContainsGenericParameters && type.GetInterfaces().Contains(typeof(IConditionalSerializer)) && ActivatorUtilities.CreateInstance(_serviceProvider, type) is IConditionalSerializer conditionalSerializer)
            {
                RegisterSerializer(conditionalSerializer);
            }

            if (type.GetInterfaces().Contains(typeof(IConditionalSerializerFactory)) && ActivatorUtilities.CreateInstance(_serviceProvider, type) is IConditionalSerializerFactory factory)
            {
                _conditionalSerializerFactories!.Add(factory);
            }
        }
    }
}