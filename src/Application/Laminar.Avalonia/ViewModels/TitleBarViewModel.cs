using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Laminar.Avalonia.Commands;
using Laminar.Avalonia.Shapes;
using Laminar.Domain.ValueObjects;
using Point = Avalonia.Point;

namespace Laminar.Avalonia.ViewModels;

public partial class TitleBarViewModel : ViewModelBase
{
    private readonly ObservableValue<string?> _toggleSettingsDescription = new();
    private readonly ObservableValue<string?> _toggleSidebarDescription = new();
    
    [ObservableProperty] 
    private bool _settingsOpen = false;

    [ObservableProperty]
    private bool _sidebarExpanded = false;
    
    public TitleBarViewModel(LaminarToolFactory toolFactory)
    {
        OnSettingsOpenChanged(SettingsOpen);
        OnSidebarExpandedChanged(SidebarExpanded);
        ToggleSettingsTool = new LaminarToolInstance
        {
            Tool = toolFactory
                .DefineTool("Settings", LaminarCommandIcon.Template(SettingsCog.GetGeometry(4.5, 8, new Size(19, 19))), 
                    _toggleSettingsDescription, new KeyGesture(Key.S, KeyModifiers.Control | KeyModifiers.Alt))
                .AsCommand(() => SettingsOpen = !SettingsOpen)
        };
        ToggleSidebarTool = new LaminarToolInstance
        {
            Tool = toolFactory
                .DefineTool("Toggle Sidebar", LaminarCommandIcon.Template(new PolylineGeometry { Points = [new Point(0, 0), new Point(10, 10), new Point(10, -10)] }), 
                    _toggleSidebarDescription, new KeyGesture(Key.B, KeyModifiers.Control))
                .AsCommand(() => SidebarExpanded = !SidebarExpanded)
        };
    }
    
    public LaminarToolInstance ToggleSettingsTool { get; }

    public LaminarToolInstance ToggleSidebarTool { get; }

    partial void OnSettingsOpenChanged(bool value)
    {
        _toggleSettingsDescription.Value = value ? "Close Settings" : "Open Settings";
    }

    partial void OnSidebarExpandedChanged(bool value)
    {
        _toggleSidebarDescription.Value = value ? "Close Sidebar" : "Open Sidebar";
    }
}