namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class Slider : IUserInterfaceDefinition
{
    public static readonly UITarget DesignInstance = new() { Name = "Default Slider", Value = 5.0 };
    
    public class UITarget : UserInterface<Slider, double>
    {
    }

    public double Max { get; init; }

    public double Min { get; init; }

    public double Increment { get; init; }
}