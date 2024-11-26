using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Input;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Implementation.Base.ActionSystem;

namespace Laminar.Avalonia.Commands;

public class LaminarCommand(
    IUserActionManager userActionManager,
    Func<object?, bool> executeAction,
    Func<object?, bool> undoAction,
    Func<object?, bool>? canExecute) : AvaloniaObject, ICommand
{
    public static readonly StyledProperty<KeyGesture?> GestureProperty =
        AvaloniaProperty.Register<LaminarCommand, KeyGesture?>(nameof(Gesture));

    public event EventHandler? CanExecuteChanged;

    public KeyGesture? Gesture
    {
        get => GetValue(GestureProperty);
        set => SetValue(GestureProperty, value);
    }
    
    public required string Name { get; init; }

    public string Description { get; init; } = "";
    
    public string Tooltip => Description + "\n" + (Gesture is not null ? $"Currently bound to {Gesture}" : "Press Alt+B to bind");
    
    public bool CanExecute(object? parameter)
        => canExecute?.Invoke(parameter) ?? true;

    public bool Execute(object? parameter)
    {
        return userActionManager.ExecuteAction(new AutoAction()
            { ExecuteAction = () => executeAction(parameter), UndoAction = () => undoAction(parameter) });
    }

    void ICommand.Execute(object? parameter)
        => Execute(parameter);
}