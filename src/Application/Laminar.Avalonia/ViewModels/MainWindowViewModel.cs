using Avalonia.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool _settingsOpen;
    [ObservableProperty] private bool _sidebarExpanded;
    
    public MainWindowViewModel(MainControlViewModel mainControl, TitleBarViewModel titlebar, SettingsViewModel settings)
    {
        MainControl = mainControl;
        TitleBar = titlebar;
        Settings = settings;
        TitleBar.MainWindow = this;
        SidebarExpanded = MainControl.SidebarExpanded;
        MainControl.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MainControl.SidebarExpanded))
                SidebarExpanded = MainControl.SidebarExpanded;
        };
    }
    
    
    public string ToggleSettingsDescription => SettingsOpen ? "Close settings" : "Open settings";

    public string ToggleSidebarDescription => SidebarExpanded ? "Collapse Sidebar" : "Expand Sidebar";
    
    public void ToggleSettings()
    {
        SettingsOpen = !SettingsOpen;
    }

    public void ToggleSidebar()
    {
        SidebarExpanded = !SidebarExpanded;
    }

    [DependsOn(nameof(SettingsOpen))]
    public bool CanToggleSidebar(object? _) => !SettingsOpen;
    
    public TitleBarViewModel TitleBar { get; }

    public SettingsViewModel Settings { get; }

    public MainControlViewModel MainControl { get; }

    partial void OnSidebarExpandedChanged(bool value)
    {
        MainControl.SidebarExpanded = value;
    }
}