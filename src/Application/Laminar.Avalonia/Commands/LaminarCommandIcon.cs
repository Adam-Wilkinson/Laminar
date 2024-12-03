using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace Laminar.Avalonia.Commands;

public class LaminarCommandIcon : TemplatedControl
{
    public static readonly StyledProperty<Geometry> IconDataProperty =
        AvaloniaProperty.Register<LaminarCommandIcon, Geometry>(nameof(IconData));

    public static readonly StyledProperty<LaminarToolInstance> CommandInstanceProperty =
        AvaloniaProperty.Register<LaminarCommandIcon, LaminarToolInstance>(nameof(ToolInstance));
    
    public Geometry IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public LaminarToolInstance ToolInstance
    {
        get => GetValue(CommandInstanceProperty);
        set => SetValue(CommandInstanceProperty, value);
    }

    public static IDataTemplate Template(Geometry iconData) => new FuncDataTemplate<LaminarToolInstance>(
        _ => true,
        commandInstance => new LaminarCommandIcon
        {
            IconData = iconData,
            ToolInstance = commandInstance,
        });
}