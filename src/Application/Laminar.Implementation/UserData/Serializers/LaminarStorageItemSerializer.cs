using Laminar.Domain.DataManagement.FileNavigation;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData.Serializers;

public class LaminarStorageItemSerializer : TypeSerializer<ILaminarStorageItem, string>
{
    protected override string SerializeTyped(ILaminarStorageItem toSerialize)
    {
        return toSerialize.Path;
    }

    protected override ILaminarStorageItem DeSerializeTyped(string serialized, object? deserializationContext = null)
    {
        return ILaminarStorageItem.FromPath(serialized);
    }
}