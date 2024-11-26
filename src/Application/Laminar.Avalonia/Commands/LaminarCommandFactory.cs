using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Domain.Notification;

namespace Laminar.Avalonia.Commands;

public class LaminarCommandFactory
{
    private readonly IUserActionManager _userActionManager;
    private readonly TopLevel _topLevel;

    public LaminarCommandFactory(IUserActionManager userActionManager, TopLevel topLevel)
    {
        _userActionManager = userActionManager;
        _topLevel = topLevel;
        _topLevel.PointerMoved += (_, e) =>
        {
            VisualUnderCursor = _topLevel.GetVisualAt(e.GetPosition(_topLevel));
        };
    }

    public Visual? VisualUnderCursor { get; private set; }

    public LaminarCommand CreateCommand(string name, 
        Func<object?, bool> execute, 
        Func<object?, bool> undo,
        Func<object?, bool>? canExecute = null,
        Func<INotifyPropertyChanged, INotificationSource>? canExecuteChanged = null,
        string description = "",
        KeyGesture? keyGesture = null)
    {
        canExecute ??= _ => true;
        var newCommand = new LaminarCommand(_userActionManager, execute, undo, canExecute, canExecuteChanged)
        {
            Name = name,
            Description = description,
            Gesture = keyGesture
        };
        
        BindCommand(newCommand);
        
        return newCommand;
    }

    public LaminarCommand CreateCommand<T>(string name,
        Func<T, bool> execute,
        Func<T, bool> undo,
        Func<T, bool>? canExecute = null,
        Func<INotifyPropertyChanged, INotificationSource>? canExecuteChanged = null,
        string description = "",
        KeyGesture? keyGesture = null)
        => CreateCommand(name, TypeCheck(execute), TypeCheck(undo), TypeCheck(canExecute), canExecuteChanged, description, keyGesture);

    private void BindCommand(LaminarCommand command)
    {
        _topLevel.KeyBindings.Add(new KeyBinding()
        {
            Command = new ExecuteLaminarCommandUnderCursor(command, this),
            [!KeyBinding.GestureProperty] = command[!LaminarCommand.GestureProperty],
        });
    }
    
    private static Func<object?, TOutput?> TypeCheck<TInput, TOutput>(Func<TInput, TOutput>? typedAction)
    {
        return obj => obj is TInput typedObject && typedAction is not null ? typedAction(typedObject) : default;
    }
}