namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class Slider : IUserInterfaceDefinition
{
    public class UITarget : UserInterface<Slider>
    {
    }

    public double Max { get; init; }

    public double Min { get; init; }

    public double Increment { get; init; }
}