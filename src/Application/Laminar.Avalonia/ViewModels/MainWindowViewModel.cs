using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Laminar.Avalonia.Views;
using Laminar.Avalonia.Views.CustomTitleBars;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly SettingsViewModel _settings = new();
    private readonly MainControlView _mainControl;
    
    [ObservableProperty] private bool _settingsOpen;
    private bool _sidebarOpen = true;
    
    public MainWindowViewModel(MainControlViewModel mainControlViewModel)
    {
        _mainControl = new() { DataContext = mainControlViewModel };
        WindowCentralControl = _mainControl;
        TitleBar.SidebarState = CurrentSidebarState();
    }

    public MainWindowViewModel() : this(App.Locator.GetRequiredService<MainControlViewModel>())
    {
    }

    public object WindowCentralControl { get; set; }

    public LaminarTitleBar TitleBar { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? new MacosTitleBar() : new WindowsTitleBar();

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

        TitleBar.SidebarState = CurrentSidebarState();
    }

    partial void OnSettingsOpenChanged(bool value)
    {
        WindowCentralControl = value ? _settings : _mainControl;
        OnPropertyChanged(nameof(WindowCentralControl));
        TitleBar.SidebarState = CurrentSidebarState();
    }

    private SidebarState CurrentSidebarState()
        => SettingsOpen ? SidebarState.Unchangeable : 
            _sidebarOpen ? SidebarState.Expanded : SidebarState.Closed;
}

public enum SidebarState
{
    Unchangeable = 0,
    Expanded = 1,
    Closed = 2,
}