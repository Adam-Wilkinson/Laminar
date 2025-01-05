namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class StringViewer : IUserInterfaceDefinition
{
    public static readonly UITarget DesignInstance = new() { Name = "Default Name", Value = "Default Value" };
    
    public class UITarget : UserInterface<StringViewer, string>
    {
    }

    public int MaxStringLength { get; set; } = 20;
}
