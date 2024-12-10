using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool _settingsOpen;
    
    public MainWindowViewModel(MainControlViewModel mainControl, TitleBarViewModel titlebar)
    {
        MainControl = mainControl;
        TitleBar = titlebar;
        TitleBar.SidebarExpanded = MainControl.SidebarExpanded;
        MainControl.PropertyChanged += (_, args) =>
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