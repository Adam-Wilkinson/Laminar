using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Laminar.Avalonia.Views.CustomTitleBars;

namespace Laminar.Avalonia.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool _settingsOpen;
    
    public MainWindowViewModel(MainControlViewModel mainControlViewModel)
    {
        MainControl = mainControlViewModel;
        TitleBar.SidebarState = CurrentSidebarState();
    }

    public LaminarTitleBar TitleBar { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? new MacosTitleBar() : new WindowsTitleBar();

    public SettingsViewModel Settings { get; } = new();

    public MainControlViewModel MainControl { get; }

    public void ToggleSidebar()
    {
        if (SettingsOpen) return;
        
        MainControl.SidebarExpanded = !MainControl.SidebarExpanded;
        TitleBar.SidebarState = CurrentSidebarState();
    }

    partial void OnSettingsOpenChanged(bool value)
    {
        TitleBar.SidebarState = CurrentSidebarState();
    }

    private SidebarState CurrentSidebarState()
        => SettingsOpen ? SidebarState.Unchangeable : 
            MainControl.SidebarExpanded ? SidebarState.Expanded : SidebarState.Closed;
}

public enum SidebarState
{
    Unchangeable = 0,
    Expanded = 1,
    Closed = 2,
}