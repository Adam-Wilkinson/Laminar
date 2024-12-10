using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Laminar.Domain.Notification;

namespace Laminar.Avalonia.ToolSystem;

public class LaminarToolSwitchIcon : TemplatedControl
{
    public static readonly StyledProperty<bool> IsOnProperty =
        AvaloniaProperty.Register<LaminarToolSwitchIcon, bool>(nameof(IsOn), defaultBindingMode: BindingMode.TwoWay);

    public bool IsOn
    {
        get => GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }

    public static IDataTemplate CreateTemplate(Func<LaminarToolInstance, IBinding> isOnBinding) =>
        new FuncDataTemplate<LaminarToolInstance>(
            _ => true,
            commandInstance => new LaminarToolSwitchIcon
            {
                [!IsOnProperty] = isOnBinding(commandInstance)
            });
}
