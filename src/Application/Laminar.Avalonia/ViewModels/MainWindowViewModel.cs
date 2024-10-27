using CommunityToolkit.Mvvm.ComponentModel;
using Laminar.Avalonia.Views;

namespace Laminar.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly SettingsViewModel _settings = new();
    private readonly MainControlView _mainControl = new();
    
    [ObservableProperty] private bool _settingsOpen;
    private bool _sidebarOpen = true;
    
    public MainWindowViewModel()
    {
        WindowCentralControl = _mainControl;
    }

    public SidebarState SidebarState => SettingsOpen
        ? SidebarState.Unchangeable
        : (_sidebarOpen ? SidebarState.Expanded : SidebarState.Closed);

    public object WindowCentralControl { get; set; }

    public void ToggleSidebar()
    {
        if (SettingsOpen) return;
        
        _sidebarOpen = !_sidebarOpen;
        if (_sidebarOpen)
        {
            _mainControl.OpenFileNavigator();
        }
        else
        {
            _mainControl.CloseFileNavigator();
        }
        OnPropertyChanged(nameof(SidebarState));
    }

    partial void OnSettingsOpenChanged(bool oldValue, bool newValue)
    {
        WindowCentralControl = newValue ? _settings : _mainControl;
        OnPropertyChanged(nameof(WindowCentralControl));
        OnPropertyChanged(nameof(SidebarState));
    }
}

public enum SidebarState
{
    Unchangeable = 0,
    Expanded = 1,
    Closed = 2,
}