namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class StringViewer : IUserInterfaceDefinition
{
    public class UITarget : UserInterface<StringViewer>
    {
    }

    public int MaxStringLength { get; set; } = 12;
}
