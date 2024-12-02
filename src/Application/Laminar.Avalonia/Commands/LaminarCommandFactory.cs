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
using Laminar.Implementation.Base.ActionSystem;
using Laminar.PluginFramework;

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
    
    public class ToolBuilder<T>(IUserActionManager actionManager, string name, ReactiveFunc<T, string> descriptionGenerator, IDataTemplate iconDataTemplate, KeyGesture gesture)
    {
        public ParameterCommand<T> AsCommand(Func<T, IUserAction> parameterAction)
            => AsCommand(item => actionManager.ExecuteAction(parameterAction(item)));

        public ParameterCommand<T> AsCommand(IUserAction action)
            => AsCommand(_ => actionManager.ExecuteAction(action));
        
        public ParameterCommand<T> AsCommand(Action execute, Action? undo)
            => undo switch
            {
                null => AsCommand(_ => execute()),
                _ => AsCommand(_ => execute(), _ => undo()) 
            };

        public ParameterCommand<T> AsCommand(Action<T> execute)
        {
            
        }
        
        public ParameterCommand<T> AsCommand(Action<T> execute, Action<T> undo)
            => AsCommand(parameter => new AutoAction
                { ExecuteAction = () => execute(parameter), UndoAction = () => undo(parameter) });

        public LaminarToolbox<T> AsToolbox(params LaminarTool<T>[] tools)
        {
            None
        }
    }
    
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