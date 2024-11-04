﻿using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Laminar.Avalonia.Views;
using Laminar.Avalonia.Views.CustomTitleBars;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool _settingsOpen;
    
    public MainWindowViewModel(MainControlViewModel mainControlViewModel)
    {
        WindowCentralControl = mainControlViewModel;
        TitleBar.SidebarState = CurrentSidebarState();
    }

    public LaminarTitleBar TitleBar { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? new MacosTitleBar() : new WindowsTitleBar();

    public SettingsViewModel Settings { get; } = new();

    public MainControlViewModel WindowCentralControl { get; }

    public void ToggleSidebar()
    {
        if (SettingsOpen) return;
        
        WindowCentralControl.SidebarExpanded = !WindowCentralControl.SidebarExpanded;
        TitleBar.SidebarState = CurrentSidebarState();
    }

    private SidebarState CurrentSidebarState()
        => SettingsOpen ? SidebarState.Unchangeable : 
            WindowCentralControl.SidebarExpanded ? SidebarState.Expanded : SidebarState.Closed;
}

public enum SidebarState
{
    Unchangeable = 0,
    Expanded = 1,
    Closed = 2,
}