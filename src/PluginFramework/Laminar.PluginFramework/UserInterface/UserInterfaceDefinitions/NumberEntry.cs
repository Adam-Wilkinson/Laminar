namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class NumberEntry : IUserInterfaceDefinition
{
    public class UITarget : UserInterface<NumberEntry>
    {
    }

    public string Units { get; set; } = "";
}
