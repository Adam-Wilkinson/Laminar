using Laminar.Domain.DataManagement.FileNavigation;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData.Serializers;

public class LaminarStorageFolderSerializer : TypeSerializer<LaminarStorageFolder, string>
{
    protected override string SerializeTyped(LaminarStorageFolder toSerialize)
    {
        return toSerialize.Path;
    }

    protected override LaminarStorageFolder DeSerializeTyped(string serialized, object? deserializationContext = null)
    {
        return new LaminarStorageFolder(serialized);
    }
}