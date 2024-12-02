using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Domain.ValueObjects;
using Laminar.Implementation.Base.ActionSystem;

namespace Laminar.Avalonia.Commands;

public class LaminarCommand(
    IUserActionManager userActionManager,
    string name,
    Func<object?, bool> execute,
    Func<object?, bool>? undo = null,
    IObservableValue<string>? descriptionObservable = null,
    IObservableValue<bool>? canExecuteObservable = null) : AvaloniaObject, ICommand
{
    public static readonly StyledProperty<KeyGesture?> GestureProperty =
        AvaloniaProperty.Register<LaminarCommand, KeyGesture?>(nameof(Gesture));
    
    private readonly IObservableValue<string> _descriptionObservable = descriptionObservable ?? new ObservableValue<string>("");
    private readonly IObservableValue<bool> _canExecuteObservable = canExecuteObservable ?? new ObservableValue<bool>(true);

    public KeyGesture? Gesture
    {
        get => GetValue(GestureProperty);
        set => SetValue(GestureProperty, value);
    }

    public string Name { get; } = name;

    public required IDataTemplate IconTemplate { get; init; }

    public IEnumerable<LaminarCommand>? ChildCommands { get; init; }

    public virtual IObservableValue<string> GetDescription(object? parameter) => _descriptionObservable;

    public virtual IObservableValue<bool> CanExecute(object? parameter) => _canExecuteObservable;
    
    bool ICommand.CanExecute(object? parameter) => CanExecute(parameter).Value;

    public void Execute(object? parameter)
    {
        if (undo is null)
        {
            execute(parameter);
        }
        else
        {
            userActionManager.ExecuteAction(new AutoAction
                { ExecuteAction = () => execute(parameter), UndoAction = () => undo(parameter) });
        }
    }

    public event EventHandler? CanExecuteChanged;
}