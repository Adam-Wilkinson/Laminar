namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class NumberEntry : IUserInterfaceDefinition
{
    public static readonly UITarget DesignInstance = new() { Name = "Default Number", Value = 5.0, Definition = new NumberEntry { Units = "ms" }};
    
    public class UITarget : UserInterface<NumberEntry, double>
    {
    }

    public string Units { get; set; } = "";
}
