using CommunityToolkit.Mvvm.ComponentModel;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;

namespace Laminar.Avalonia.ViewModels;
public partial class MainControlViewModel(FileNavigatorViewModel fileNavigator) : ViewModelBase
{
    [ObservableProperty] private bool _sidebarExpanded = true;

    [ObservableProperty, Serialize] private double _sidebarWidth = 350;
    
    public FileNavigatorViewModel FileNavigator { get; } = fileNavigator;
    
    partial void OnSidebarWidthChanged(double value)
    {
        OnPropertyChanged("SidebarWidth");
    }
}