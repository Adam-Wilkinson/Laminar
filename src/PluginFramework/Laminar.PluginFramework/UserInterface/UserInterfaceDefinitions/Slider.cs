namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class Slider : IUserInterfaceDefinition
{
    public class UITarget : UserInterface<Slider>
    {
    }

    public Slider(double min, double max)
    {
        Max = max;
        Min = min;
    }

    public double Max { get; }

    public double Min { get; }

    public double Increment { get; init; }
}