using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.VisualTree;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Domain.Notification;
using Laminar.Domain.ValueObjects;

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

    public LaminarCommand CreateCommand(
        string name,
        Func<bool> execute,
        Func<bool> undo,
        IDataTemplate iconDataTemplate,
        IObservableValue<string> description,
        IObservableValue<bool>? canExecute = null,
        KeyGesture? gesture = null,
        IEnumerable<LaminarCommand>? children = null)
    {
        var result = new LaminarCommand(_userActionManager, name, _ => execute(), _ => undo(), description, canExecute)
        {
            Gesture = gesture,
            IconTemplate = iconDataTemplate,
            ChildCommands = children
        };
        
        BindCommand(result);

        return result;
    }

    public ParameterCommand<T> CreateParameterCommand<T>(
        string name,
        Func<T, bool> execute,
        Func<T, bool>? undo,
        IDataTemplate iconTemplate,
        ReactiveFunc<T, string> descriptionFactory,
        ReactiveFunc<T, bool>? canExecute = null,
        KeyGesture? gesture = null,
        IEnumerable<ParameterCommand<T>>? children = null)
    {
        var result = new ParameterCommand<T>(_userActionManager, name, execute, undo, canExecute ?? new ReactiveFunc<T, bool>(_ => true), descriptionFactory)
        {
            Gesture = gesture,
            IconTemplate = iconTemplate,
            ChildCommands = children
        };
        
        BindCommand(result);

        return result;
    }
    
    private void BindCommand(LaminarCommand command)
    {
        _topLevel.KeyBindings.Add(new KeyBinding()
        {
            Command = new ExecuteCommandUnderCursor(command, this),
            [!KeyBinding.GestureProperty] = command[!LaminarCommand.GestureProperty],
        });
    }
}