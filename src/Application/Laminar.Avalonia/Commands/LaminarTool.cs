using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Domain.Notification;
using Laminar.Domain.ValueObjects;

namespace Laminar.Avalonia.Commands;

public class Toolbox<T>(IEnumerable<LaminarTool<T>> childTools, ReactiveFunc<T, string> descriptionTemplate) : LaminarTool<T>(descriptionTemplate)
{
    public override IEnumerable<LaminarTool<T>>? ChildTools { get; } = childTools;

    public override IObservableValue<bool> CanExecute(object? parameter) => new ObservableValue<bool>(parameter is T);
}

public class Toolbox(IEnumerable<LaminarTool> commands, IObservableValue<string> descriptionObservable)
    : LaminarTool(descriptionObservable)
{
    public override IEnumerable<LaminarTool>? ChildTools { get; } = commands;
}

public class Command(Action commandAction, IObservableValue<string> descriptionObservable, IObservableValue<bool>? canExecuteObservable) : LaminarTool(descriptionObservable)
{
    public override IObservableValue<bool> CanExecute(object? _) =>
        canExecuteObservable ?? new ObservableValue<bool>(false);

    public override void Execute(object? parameter)
    {
        commandAction?.Invoke();
    }
}

public class Command<TParameter>(
    IParameterAction<TParameter> action,
    IUserActionManager actionManager,
    ReactiveFunc<TParameter, string> descriptionTemplate) : LaminarTool<TParameter>(descriptionTemplate)
{
    public override IObservableValue<bool> CanExecute(object? parameter) => parameter is TParameter typed 
        ? action.CanExecute(typed)
        : new ObservableValue<bool>(false);

    public override void Execute(object? parameter)
    {
        if (parameter is TParameter typed)
        {
            actionManager.ExecuteAction(action.WithParameter(typed));
        }
    }
}

public abstract class LaminarTool<T>(ReactiveFunc<T, string> descriptionTemplate) : LaminarTool(null)
{
    public override IObservableValue<string> GetDescription(object? parameter) => parameter is T typed 
        ? descriptionTemplate.GetObservable(typed)
        : new ObservableValue<string>("Invalid Parameter Type");

    public override IEnumerable<LaminarTool<T>>? ChildTools => null;
}

public abstract class LaminarTool(IObservableValue<string>? descriptionObservable) : AvaloniaObject, ICommand
{
    public static readonly StyledProperty<KeyGesture?> GestureProperty =
        AvaloniaProperty.Register<LaminarTool, KeyGesture?>(nameof(Gesture));
    
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