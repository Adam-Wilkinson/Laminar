using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Laminar.Domain.ValueObjects;

namespace Laminar.Avalonia.Commands;

public class Command<T> : LaminarTool<T>
{
    
}

public abstract class LaminarTool<T>(IObservableValue<string>? descriptionObservable) : LaminarTool(descriptionObservable)
{
    public override IEnumerable<LaminarTool<T>>? ChildTools => null;
}

public abstract class LaminarTool(IObservableValue<string>? descriptionObservable) : AvaloniaObject, ICommand
{
    public static readonly StyledProperty<KeyGesture?> GestureProperty =
        AvaloniaProperty.Register<LaminarCommand, KeyGesture?>(nameof(Gesture));
    
    private readonly IObservableValue<string> _descriptionObservable = descriptionObservable ?? new ObservableValue<string>("");
    private readonly ObservableValue<bool> _canExecuteObservable = new(false);
    
    public KeyGesture? Gesture
    {
        get => GetValue(GestureProperty);
        set => SetValue(GestureProperty, value);
    }

    public required string Name { get; init; }

    public required IDataTemplate IconTemplate { get; init; }

    public virtual IObservableValue<string> GetDescription(object? _) => _descriptionObservable;

    public virtual IEnumerable<LaminarTool>? ChildTools => null;

    public virtual IObservableValue<bool> CanExecute(object? _) => _canExecuteObservable;
    
    bool ICommand.CanExecute(object? parameter) => CanExecute(parameter).Value;

    public virtual void Execute(object? parameter)
    {
    }
    
    public event EventHandler? CanExecuteChanged;
}