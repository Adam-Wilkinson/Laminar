namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class ToggleSwitch : IUserInterfaceDefinition
{
    public static readonly UITarget DesignInstance = new() { Name = "Default Name", Value = true };
    
    public class UITarget : UserInterface<ToggleSwitch, bool>
    {
    }
}
