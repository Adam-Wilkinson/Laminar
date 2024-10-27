using Avalonia;
using Avalonia.Controls;
using Laminar.Avalonia.ViewModels;

namespace Laminar.Avalonia.Views.CustomTitleBars;

public class LaminarTitleBar : UserControl
{
    public static readonly StyledProperty<SidebarState> SidebarStateProperty =
        AvaloniaProperty.Register<LaminarTitleBar, SidebarState>(nameof(SidebarState));
    
    public SidebarState SidebarState
    {
        get => GetValue(SidebarStateProperty);
        set => SetValue(SidebarStateProperty, value);
    }
}