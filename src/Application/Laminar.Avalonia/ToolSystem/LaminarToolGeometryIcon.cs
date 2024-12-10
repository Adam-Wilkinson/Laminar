using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace Laminar.Avalonia.ToolSystem;

public class LaminarToolGeometryIcon : TemplatedControl
{
    public static readonly StyledProperty<Geometry> IconDataProperty =
        AvaloniaProperty.Register<LaminarToolGeometryIcon, Geometry>(nameof(IconData));

    public static readonly StyledProperty<LaminarToolInstance> CommandInstanceProperty =
        AvaloniaProperty.Register<LaminarToolGeometryIcon, LaminarToolInstance>(nameof(ToolInstance));
    
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

    public static IDataTemplate CreateTemplate(Geometry iconData) => new FuncDataTemplate<LaminarToolInstance>(
        _ => true,
        commandInstance => new LaminarToolGeometryIcon
        {
            IconData = iconData,
            ToolInstance = commandInstance,
        });
}