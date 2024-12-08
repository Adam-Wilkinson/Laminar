using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Laminar.Avalonia.Commands;
using Laminar.Avalonia.Shapes;
using Laminar.Avalonia.Views.CustomTitleBars;

namespace Laminar.Avalonia.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool _settingsOpen;
    
    public MainWindowViewModel(MainControlViewModel mainControl, TitleBarViewModel titlebar)
    {
        MainControl = mainControl;
        TitleBar = titlebar;
        TitleBar.SidebarExpanded = MainControl.SidebarExpanded;
        MainControl.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(MainControl.SidebarExpanded))
                TitleBar.SidebarExpanded = MainControl.SidebarExpanded;
        };
        TitleBar.PropertyChanged += (_, args) =>
        {
            switch (args.PropertyName)
            {
                case nameof(TitleBar.SidebarExpanded):
                    MainControl.SidebarExpanded = TitleBar.SidebarExpanded;
                    break;
                case nameof(TitleBar.SettingsOpen):
                    SettingsOpen = TitleBar.SettingsOpen;
                    break;
            }
        };
    }
    
    public TitleBarViewModel TitleBar { get; }

    public SettingsViewModel Settings { get; } = new();

    public MainControlViewModel MainControl { get; }
}

public enum SidebarState
{
    Unchangeable = 0,
    Expanded = 1,
    Closed = 2,
}