namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class DefaultViewer : IUserInterfaceDefinition
{
    public static readonly UITarget DesignInstance = new() { Name = "Default Name", Value = None.Instance};
    
    public class UITarget : UserInterface<DefaultViewer, None>
    {
    }
}