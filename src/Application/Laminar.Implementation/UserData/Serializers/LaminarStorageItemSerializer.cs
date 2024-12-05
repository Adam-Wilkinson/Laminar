using System;
using System.Linq;
using Laminar.Contracts.UserData.FileNavigation;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData.Serializers;

public class LaminarStorageItemSerializerFactory(ILaminarStorageItemFactory factory) : IConditionalSerializerFactory
{
    public IConditionalSerializer? TryCreateSerializerFor(Type type)
        => !type.GetInterfaces().Contains(typeof(ILaminarStorageItem))
            ? null
            : (IConditionalSerializer)Activator.CreateInstance(
                typeof(LaminarStorageItemSerializer<>).MakeGenericType(type), factory);
}

public class LaminarStorageItemSerializer<T>(ILaminarStorageItemFactory factory) : TypeSerializer<T, string> 
    where T : class, ILaminarStorageItem
{
    protected override string SerializeTyped(T toSerialize)
        => toSerialize.Path;

    protected override T DeSerializeTyped(string serialized, object? deserializationContext = null)
        => factory.FromPath<T>(serialized);
}