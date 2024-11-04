using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Laminar.Avalonia.AdjustableStackPanel;
using Laminar.Avalonia.DragDrop;
using Laminar.Avalonia.ViewModels;

namespace Laminar.Avalonia.Views;

public partial class MainControlView : UserControl
{
    public static readonly StyledProperty<bool> SidebarOpenProperty = AvaloniaProperty.Register<MainControlView, bool>(nameof(SidebarOpen));

    public bool SidebarOpen
    {
        get => GetValue(SidebarOpenProperty);
        set => SetValue(SidebarOpenProperty, value);
    }
    
    private readonly DoubleTransition _opacityTransition = new() { Property = OpacityProperty };
    private double _lastFileNavigatorSize = -1;

    static MainControlView()
    {
        SidebarOpenProperty.Changed.AddClassHandler<MainControlView, bool>((o, e) => o.SidebarStateChange(e));
    }
    
    public MainControlView()
    {
        InitializeComponent();
    }

    private void SidebarStateChange(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (!e.NewValue.HasValue) return;

        if (e.NewValue.Value)
        {
            OpenFileNavigator();
        }
        else
        {
            CloseFileNavigator();
        }
    }
    
    private void CloseFileNavigator()
    {
        FileNavigator.Transitions ??= [];
        _opacityTransition.Duration = SidebarStackPanel.TransitionDuration;
        FileNavigator.Transitions.Add(_opacityTransition);
        _lastFileNavigatorSize = ResizeWidget.GetOrCreateResizer(FileNavigator).Size;

        FileNavigator.ClipToBounds = true;
        FileNavigator.Opacity = 0;
        ResizeWidget.GetOrCreateResizer(FileNavigator).SetSizeTo(0, true);
    }

    private async void OpenFileNavigator()
    {
        if (_lastFileNavigatorSize < 0)
        {
            return;
        }
        
        ResizeWidget.GetOrCreateResizer(FileNavigator).SetSizeTo(_lastFileNavigatorSize, true);
        FileNavigator.Opacity = 1;

        await Task.Delay(SidebarStackPanel.TransitionDuration);
        
        FileNavigator.ClipToBounds = false;
        FileNavigator.Transitions?.Remove(_opacityTransition);
    }
}