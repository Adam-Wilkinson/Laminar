using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Laminar.Domain.Notification;
using Laminar.Domain.ValueObjects;

namespace Laminar.Avalonia.ToolSystem;

public abstract class LaminarTool<T>(ReactiveFunc<T, string?> descriptionTemplate) : LaminarTool(null)
{
    public override IObservableValue<string?> GetDescription(object? parameter) => parameter is T typed 
        ? descriptionTemplate.GetObservable(typed)
        : new ObservableValue<string?>(null);

    public override IEnumerable<LaminarTool<T>>? ChildTools => null;
}

public class BindingTestClass : AvaloniaObject
{
    public static readonly StyledProperty<IBinding> BindingTestProperty =
        AvaloniaProperty.Register<LaminarTool, IBinding>(nameof(BindingTest));

    public IBinding BindingTest
    {
        get => GetValue(BindingTestProperty);
        set => SetValue(BindingTestProperty, value);
    }

    static BindingTestClass()
    {
        BindingTestProperty.Changed.AddClassHandler<AvaloniaObject>((property, args) =>
        {
            Debug.WriteLine("Huh");
        });
    }
}

public abstract class LaminarTool(IObservableValue<string?>? descriptionObservable) : AvaloniaObject, ICommand
{
    public static readonly StyledProperty<KeyGesture?> GestureProperty =
        AvaloniaProperty.Register<LaminarTool, KeyGesture?>(nameof(Gesture));
    
    private readonly IObservableValue<string?> _descriptionObservable = descriptionObservable ?? new ObservableValue<string?>(null);
    private readonly ObservableValue<bool> _canExecuteObservable = new(false);
    
    public KeyGesture? Gesture
    {
        get => GetValue(GestureProperty);
        set => SetValue(GestureProperty, value);
    }

    public required string Name { get; init; }

    public required IDataTemplate IconTemplate { get; init; }

    public virtual IObservableValue<string?> GetDescription(object? _) => _descriptionObservable;

    public virtual IEnumerable<LaminarTool>? ChildTools => null;
    
    public virtual IObservableValue<bool> CanExecute(object? _) => _canExecuteObservable;
    
    bool ICommand.CanExecute(object? parameter) => CanExecute(parameter).Value;

    public virtual void Execute(object? parameter)
    {
    }
    
    public event EventHandler? CanExecuteChanged;
}