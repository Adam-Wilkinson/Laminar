namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class Checkbox : IUserInterfaceDefinition
{
    public static readonly UITarget DesignInstance = new() { Value = true, Name = "Test Checkbox" };
    
    public class UITarget : UserInterface<Checkbox>
    {
    }
}
