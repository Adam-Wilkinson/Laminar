using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Laminar.Contracts.Base.ActionSystem;

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
        string description = "",
        KeyGesture? keyGesture = null)
    {
        var newCommand = new LaminarCommand(_userActionManager, execute, undo, canExecute)
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
        string description = "",
        KeyGesture? keyGesture = null)
        => CreateCommand(name, TypeCheck(execute), TypeCheck(undo), TypeCheck(canExecute), description, keyGesture);

    private void BindCommand(LaminarCommand command)
    {
        _topLevel.KeyBindings.Add(new KeyBinding()
        {
            Command = new ExecuteLaminarCommandUnderCursor(command, this),
            [!KeyBinding.GestureProperty] = command[!LaminarCommand.GestureProperty],
        });
    }
    
    private static Func<object?, bool> TypeCheck<T>(Func<T, bool>? typedAction)
    {
        return obj => obj is T typedObject && (typedAction is null || typedAction(typedObject));
    }
}