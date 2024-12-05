using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;
public partial class MainControlViewModel : ViewModelBase
{
    [ObservableProperty, Serialize] private bool _sidebarExpanded = true;

    [ObservableProperty, Serialize] private double _expandedSidebarWidth = 350;

    [ObservableProperty] private double _currentSidebarWidth;

    public MainControlViewModel(FileNavigatorViewModel fileNavigator)
    {
        FileNavigator = fileNavigator;
        OnExpandedSidebarWidthChanged(_expandedSidebarWidth);
    }

    public FileNavigatorViewModel FileNavigator { get; }

    partial void OnSidebarExpandedChanged(bool value)
    {
        CurrentSidebarWidth = value ? ExpandedSidebarWidth : 0;
    }

    partial void OnExpandedSidebarWidthChanged(double value)
    {
        if (SidebarExpanded) CurrentSidebarWidth = value;
    }

    partial void OnCurrentSidebarWidthChanged(double value)
    {
        if (SidebarExpanded) ExpandedSidebarWidth = value;
    }
}