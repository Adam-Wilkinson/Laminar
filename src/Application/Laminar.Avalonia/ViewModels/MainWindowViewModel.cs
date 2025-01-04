﻿using CommunityToolkit.Mvvm.ComponentModel;

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
    
    public TitleBarViewModel TitleBar { get; }

    public SettingsViewModel Settings { get; }

    public MainControlViewModel MainControl { get; }

    partial void OnSidebarExpandedChanged(bool value)
    {
        MainControl.SidebarExpanded = value;
    }
}