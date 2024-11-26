using Avalonia;

namespace Laminar.Avalonia.Commands;

public class LaminarCommandWithParameter : AvaloniaObject
{
    public static readonly StyledProperty<LaminarCommandVisual> CommandVisualProperty =
        AvaloniaProperty.Register<LaminarCommandWithParameter, LaminarCommandVisual>(nameof(CommandVisual));
    
    public static readonly StyledProperty<object?> ParameterProperty = 
        AvaloniaProperty.Register<LaminarCommandWithParameter, object?>(nameof(Parameter));

    public LaminarCommandVisual CommandVisual
    {
        get => GetValue(CommandVisualProperty);
        set => SetValue(CommandVisualProperty, value);
    }

    public object? Parameter
    {
        get => GetValue(ParameterProperty);
        set => SetValue(ParameterProperty, value);
    }

    public bool Execute() => CommandVisual.Command.CanExecute(Parameter) && CommandVisual.Command.Execute(Parameter);
}