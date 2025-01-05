using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace Laminar.PluginFramework.UserInterface;
public interface IValueInterfaceDefinition
{
    public Type? ValueType { get; }

    public bool IsUserEditable { get; set; }

    public IUserInterfaceDefinition? Editor { get; set; }

    public IUserInterfaceDefinition? Viewer { get; set; }

    public IUserInterfaceDefinition? GetCurrentDefinition();
}

public class DisplayValue<T> : DisplayValue
{
    public T Value { get; set; }
}

public class DisplayValue
{
    public Type ValueType { get; }

    public bool IsUserEditable { get; set; }

    public ValueInterface Interface { get; }
}

public class SliderInterfaceDefinition : IUserInterfaceDefinition
{
    public class UITarget : UserInterface<SliderInterfaceDefinition>
    {
    }

    public double Max { get; init; }

    public double Min { get; init; }
}

public class UserInterface<T> : ValueInterface where T : IUserInterfaceDefinition, new()
{
    public T Definition { get; init; } = new();
}

public class ValueInterface
{
    public string Name { get; init; } = "";

    public object? Value { get; init; }
}