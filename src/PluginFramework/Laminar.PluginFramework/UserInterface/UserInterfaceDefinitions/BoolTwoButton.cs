namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class BoolTwoButton : IUserInterfaceDefinition
{
    public static readonly UITarget DesignInstance = new() { Name = "Default boolean", Value = true };
    
    public class UITarget : UserInterface<BoolTwoButton, bool>
    {
    }

    public string TrueText { get; init; } = "True";

    public string FalseText { get; init; } = "False";
}