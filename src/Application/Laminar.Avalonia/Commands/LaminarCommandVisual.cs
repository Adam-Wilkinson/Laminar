using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;

namespace Laminar.Avalonia.Commands;

public class LaminarCommandIcon : TemplatedControl
{
    public static readonly StyledProperty<Geometry> IconDataProperty =
        AvaloniaProperty.Register<LaminarCommandIcon, Geometry>(nameof(IconData));

    public static readonly StyledProperty<LaminarCommandWithParameter> CommandProperty =
        AvaloniaProperty.Register<LaminarCommandIcon, LaminarCommandWithParameter>(nameof(Command));
    
    public Geometry IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public LaminarCommandWithParameter Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}

public class LaminarCommandSwitch : TemplatedControl
{
    public static readonly StyledProperty<bool> IsOnProperty =
        AvaloniaProperty.Register<LaminarCommandSwitch, bool>(nameof(IsOn), defaultBindingMode: BindingMode.TwoWay);

    public bool IsOn
    {
        get => GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }
}

public class LaminarCommandVisual : AvaloniaObject
{
    public static readonly StyledProperty<LaminarCommand> CommandProperty = AvaloniaProperty.Register<LaminarCommandVisual, LaminarCommand>(nameof(Command));

    public static readonly StyledProperty<IDataTemplate> DataTemplateProperty =
        AvaloniaProperty.Register<LaminarCommandVisual, IDataTemplate>(nameof(DataTemplate));

    public required LaminarCommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public IDataTemplate DataTemplate
    {
        get => GetValue(DataTemplateProperty);
        set => SetValue(DataTemplateProperty, value);
    }
}

public static class LaminarCommandVisualExtensions
{
    public static LaminarCommandVisual WithIcon(this LaminarCommand command, Geometry iconData)
        => command.WithVisual(commandWithParameter => new LaminarCommandIcon
        {
            IconData = iconData,
            Command = commandWithParameter,
        });

    public static LaminarCommandVisual WithSwitch(this LaminarCommand command, Func<LaminarCommandWithParameter, IBinding> onOffBindingFunc)
        => command.WithVisual(commandWithParameter => new LaminarCommandSwitch
        {
            [!LaminarCommandSwitch.IsOnProperty] = onOffBindingFunc(commandWithParameter),
        });

    public static LaminarCommandVisual WithVisual(this LaminarCommand command,
        Func<LaminarCommandWithParameter, Control> controlFactory)
        => new()
        {
            Command = command,
            DataTemplate = new FuncDataTemplate<LaminarCommandWithParameter?>(
                _ => true, 
                controlFactory!)
        };
}