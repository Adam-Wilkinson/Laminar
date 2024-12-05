using Laminar.Contracts.UserData.FileNavigation;
using Laminar.Implementation.UserData.FileNavigation;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData.Serializers;

public class LaminarStorageItemSerializer(ILaminarStorageItemFactory factory) : TypeSerializer<ILaminarStorageFolder, string>
{
    protected override string SerializeTyped(ILaminarStorageFolder toSerialize)
    {
        return toSerialize.Path;
    }

    protected override ILaminarStorageFolder DeSerializeTyped(string serialized, object? deserializationContext = null)
    {
        return factory.FromPath<LaminarStorageFolder>(serialized);
    }
}