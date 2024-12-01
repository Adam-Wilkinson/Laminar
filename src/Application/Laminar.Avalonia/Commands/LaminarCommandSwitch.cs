using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Laminar.Domain.Notification;

namespace Laminar.Avalonia.Commands;

public class LaminarCommandSwitch : TemplatedControl
{
    public static readonly StyledProperty<bool> IsOnProperty =
        AvaloniaProperty.Register<LaminarCommandSwitch, bool>(nameof(IsOn), defaultBindingMode: BindingMode.TwoWay);

    public bool IsOn
    {
        get => GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }

    public static IDataTemplate Template(Func<LaminarCommandInstance, IBinding> isOnBinding) =>
        new FuncDataTemplate<LaminarCommandInstance>(
            _ => true,
            commandInstance => new LaminarCommandSwitch
            {
                [!IsOnProperty] = isOnBinding(commandInstance)
            });
}
