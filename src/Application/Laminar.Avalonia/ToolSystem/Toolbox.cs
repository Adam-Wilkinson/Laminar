using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using Laminar.Avalonia.ViewModels;
using Laminar.Domain.Notification;
using Laminar.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Avalonia.ToolSystem;

public class Toolbox<T>(IEnumerable<LaminarTool<T>> childTools, ReactiveFunc<T, string?> descriptionTemplate) : LaminarTool<T>(descriptionTemplate)
{
    public override IEnumerable<LaminarTool<T>>? ChildTools { get; } = childTools;

    public override IObservableValue<bool> CanExecute(object? parameter) => new ObservableValue<bool>(parameter is T);
}

public class Toolbox(IEnumerable<LaminarTool> commands, IObservableValue<string?> descriptionObservable)
    : LaminarTool(descriptionObservable)
{
    public static readonly AttachedProperty<Toolbox> ToolboxProperty = AvaloniaProperty.RegisterAttached<Toolbox, Control, Toolbox>("Toolbox");
    
    public static Toolbox GetToolbox(Control control) => control.GetValue(ToolboxProperty);
    public static void SetToolbox(Control control, Toolbox toolbox) => control.SetValue(ToolboxProperty, toolbox);

    static Toolbox()
    {
        ToolboxProperty.Changed.AddClassHandler<Control>((control, args) =>
        {
            ((IServiceProvider)control.FindResource("ServiceProvider")!).GetRequiredService<LaminarToolFactory>();
        });
    }
    
    [Content]
    public override IEnumerable<LaminarTool>? ChildTools { get; } = commands;
}