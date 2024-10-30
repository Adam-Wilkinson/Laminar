using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Laminar.Avalonia.AdjustableStackPanel;
using Laminar.Avalonia.DragDrop;
using Laminar.Avalonia.ViewModels;

namespace Laminar.Avalonia.Views;

public partial class MainControlView : UserControl
{
    private readonly DoubleTransition _opacityTransition = new() { Property = OpacityProperty };
    private double _lastFileNavigatorSize = -1;

    public MainControlView()
    {
        InitializeComponent();
    }

    public void CloseFileNavigator()
    {
        FileNavigator.Transitions ??= [];
        _opacityTransition.Duration = SidebarStackPanel.TransitionDuration;
        FileNavigator.Transitions.Add(_opacityTransition);
        _lastFileNavigatorSize = ResizeWidget.GetOrCreateResizer(FileNavigator).Size;

        FileNavigator.ClipToBounds = true;
        FileNavigator.Opacity = 0;
        ResizeWidget.GetOrCreateResizer(FileNavigator).SetSizeTo(0, true);
    }

    public async void OpenFileNavigator()
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

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is not MainControlViewModel vm)
        {
            return;
        }

        vm.PropertyChanged += DataContext_PropertyChanged;
        DataContext_PropertyChanged(vm, new PropertyChangedEventArgs(nameof(MainControlViewModel.SidebarExpanded)));
    }

    private void DataContext_PropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName != nameof(MainControlViewModel.SidebarExpanded) || DataContext is not MainControlViewModel vm) return;
        if (vm.SidebarExpanded)
        {
            OpenFileNavigator();
        }
        else
        {
            CloseFileNavigator();
        }
    }

    private void DropHandler_OnDrop(object? sender, DragEventArgs e)
    {
        Debug.WriteLine($"Control {e.DraggingVisual} was dragged onto {e.HoverOverInteractive}, event {e.RoutedEvent!.Name}");
        e.Handled = true;
    }
}