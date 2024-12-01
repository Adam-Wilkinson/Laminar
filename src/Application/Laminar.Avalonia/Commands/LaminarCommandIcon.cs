using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace Laminar.Avalonia.Commands;

public class LaminarCommandIcon : TemplatedControl
{
    public static readonly StyledProperty<Geometry> IconDataProperty =
        AvaloniaProperty.Register<LaminarCommandIcon, Geometry>(nameof(IconData));

    public static readonly StyledProperty<LaminarCommandInstance> CommandInstanceProperty =
        AvaloniaProperty.Register<LaminarCommandIcon, LaminarCommandInstance>(nameof(CommandInstance));
    
    public Geometry IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public LaminarCommandInstance CommandInstance
    {
        get => GetValue(CommandInstanceProperty);
        set => SetValue(CommandInstanceProperty, value);
    }

    public static IDataTemplate Template(Geometry iconData) => new FuncDataTemplate<LaminarCommandInstance>(
        _ => true,
        commandInstance => new LaminarCommandIcon
        {
            IconData = iconData,
            CommandInstance = commandInstance,
        });
}