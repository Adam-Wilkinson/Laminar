﻿using Avalonia.Controls;
using BasicFunctionality.Avalonia.UserControls;
using Laminar.PluginFramework.Registration;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;
using Slider = Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions.Slider;
using StringEditor = Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions.StringEditor;
using ToggleSwitch = Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions.ToggleSwitch;

[module: HasFrontendDependency(FrontendDependency.Avalonia)]

namespace BasicFunctionality.Avalonia;

public class PluginFront : IPlugin
{
    public Platforms Platforms => Platforms.All;

    public string PluginName => "Basic Functionality UI";

    public string PluginDescription => "Basic user interface elements for the Avalonia frontend";

    public void Register(IPluginHost host)
    {
        host.RegisterInterface<NumberEntry, NumberEditor, Control>();
        host.RegisterInterface<DefaultViewer, DefaultDisplay, Control>();
        host.RegisterInterface<StringViewer, StringDisplay, Control>();
        host.RegisterInterface<Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions.StringEditor, UserControls.StringEditor, Control>();
        host.RegisterInterface<EnumDropdown, EnumEditor, Control>();
        host.RegisterInterface<Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions.Slider, SliderEditor, Control>();
        host.RegisterInterface<Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions.ToggleSwitch, UserControls.ToggleSwitch, Control>();
        host.RegisterInterface<Checkbox, UserControls.CheckBox, Control>();
        host.RegisterInterface<EditableLabel, AvaloniaEditableLabel, Control>();

        host.RegisterDataInterface<NumberEntry, double, NumberEditor>();
        host.RegisterDataInterface<DefaultViewer, object, DefaultDisplay>();
        host.RegisterDataInterface<StringViewer, string, StringDisplay>();
        host.RegisterDataInterface<StringEditor, string, UserControls.StringEditor>();
        host.RegisterDataInterface<EnumDropdown, object, EnumEditor>();
        host.RegisterDataInterface<Slider, double, SliderEditor>();
        host.RegisterDataInterface<ToggleSwitch, bool, UserControls.ToggleSwitch>();
        host.RegisterDataInterface<Checkbox, bool, UserControls.CheckBox>();
        host.RegisterDataInterface<EditableLabel, string, AvaloniaEditableLabel>();
    }
}
