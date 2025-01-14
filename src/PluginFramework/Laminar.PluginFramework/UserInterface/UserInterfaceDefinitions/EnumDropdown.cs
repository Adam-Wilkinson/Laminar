using System.Collections.Specialized;

namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class EnumDropdown : IUserInterfaceDefinition
{
    public static readonly UITarget Default = new() { Value = NotifyCollectionChangedAction.Add, Name = "Default Enum"};
    
    public class UITarget : InterfaceData<EnumDropdown, object>
    {
    }
}
