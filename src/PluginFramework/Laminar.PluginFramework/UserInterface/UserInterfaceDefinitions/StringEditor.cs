namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class StringEditor : IUserInterfaceDefinition
{
    public static readonly UITarget DesignInstance = new() { Name = "Default String", Value = "Default Value" };
    
    public class UITarget : InterfaceData<StringEditor, string>
    {
    }
}
