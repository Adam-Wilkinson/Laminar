using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;
public partial class MainControlViewModel(FileNavigatorViewModel fileNavigator) : ViewModelBase
{
    [ObservableProperty, Serialize] private bool _sidebarExpanded = true;

    [ObservableProperty, Serialize] private double _expandedSidebarWidth = 350;

    [ObservableProperty] private double _currentSidebarWidth;
    
    public FileNavigatorViewModel FileNavigator { get; } = fileNavigator;

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