using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Layout;
using Laminar.Avalonia.Views;

namespace Laminar.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly SettingsOverlay _settingsOverlay = new();
    private readonly MainControl _laminarEditor = new();
    
    private bool _settingsOpen = false;
    private bool _sidebarOpen = true;
    
    public MainWindowViewModel()
    {
        WindowCentralControl = _laminarEditor;
    }

    public SidebarState SidebarState => _settingsOpen
        ? SidebarState.Unchangeable
        : (_sidebarOpen ? SidebarState.Expanded : SidebarState.Closed);

    public Control WindowCentralControl { get; set; }

    public bool SettingsOpen
    {
        get => _settingsOpen;
        set
        {
            if (_settingsOpen == value) return;
            
            _settingsOpen = value;
            WindowCentralControl = _settingsOpen ? _settingsOverlay : _laminarEditor;
            OnPropertyChanged(nameof(WindowCentralControl));
            OnPropertyChanged(nameof(SidebarState));
            OnPropertyChanged();
        }
    }

    public void ToggleSidebar()
    {
        if (!_settingsOpen)
        {
            _sidebarOpen = !_sidebarOpen;
            OnPropertyChanged(nameof(SidebarState));
        }
    }
}

public enum SidebarState
{
    Unchangeable = 0,
    Expanded = 1,
    Closed = 2,
}