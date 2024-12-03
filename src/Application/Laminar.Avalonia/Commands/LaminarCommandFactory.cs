using System;
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

    public ToolBuilder DefineTool(string name, string description, IDataTemplate iconTemplate, KeyGesture? gesture = null)
        => DefineTool(name, new ObservableValue<string>(description), iconTemplate, gesture); 
    
    public ToolBuilder DefineTool(string name, IObservableValue<string> description, IDataTemplate iconTemplate,
        KeyGesture? gesture = null)
        => new(this, name, description, iconTemplate, gesture);

    public ToolBuilder<T> DefineTool<T>(string name, string description, IDataTemplate iconTemplate,
        KeyGesture? gesture = null)
        => DefineTool(name, new ReactiveFunc<T, string>(_ => description), iconTemplate, gesture);
    
    public ToolBuilder<T> DefineTool<T>(string name, Func<T, string> autoDescriptionGenerator, IDataTemplate iconTemplate, KeyGesture? gesture = null)
        => DefineTool(name, new ReactiveFunc<T, string>(autoDescriptionGenerator), iconTemplate, gesture);
    
    public ToolBuilder<T> DefineTool<T>(string name, ReactiveFunc<T, string> descriptionGenerator,
        IDataTemplate iconTemplate, KeyGesture? gesture = null) 
        => new(this, name, descriptionGenerator, iconTemplate, gesture);

    public class ToolBuilder(
        LaminarCommandFactory factory,
        string name,
        IObservableValue<string> descriptionObservable,
        IDataTemplate iconDataTemplate,
        KeyGesture? gesture)
    {
        public Command AsCommand(Action action, IObservableValue<bool>? canExecute = null)
            => factory.BindTool(new Command(action, descriptionObservable, canExecute ?? new ObservableValue<bool>(true))
            {
                Name = name,
                IconTemplate = iconDataTemplate,
                Gesture = gesture,
            });

        public Command AsCommand(IUserAction action)
            => factory.BindTool(new Command(() => factory._userActionManager.ExecuteAction(action), descriptionObservable, action.CanExecuteObservable())
            {
                Name = name,
                IconTemplate = iconDataTemplate,
                Gesture = gesture,
            });
        
        public Toolbox AsToolbox(params LaminarTool[] children)
            => factory.BindTool(new Toolbox(children, descriptionObservable)
            {
                Name = name,
                IconTemplate = iconDataTemplate,
                Gesture = gesture,
            });
    }
    
    public class ToolBuilder<TParameter>(
        LaminarCommandFactory factory,
        string name, 
        ReactiveFunc<TParameter, string> descriptionGenerator, 
        IDataTemplate iconDataTemplate, 
        KeyGesture? gesture)
    {
        public Command<TParameter> AsCommand(IParameterAction<TParameter> action)
            => factory.BindTool(new Command<TParameter>(action, factory._userActionManager, descriptionGenerator)
            {
                Name = name,
                IconTemplate = iconDataTemplate,
                Gesture = gesture
            });

        public Toolbox<TParameter> AsToolbox(params LaminarTool<TParameter>[] children)
            => factory.BindTool(new Toolbox<TParameter>(children, descriptionGenerator)
            {
                Name = name,
                IconTemplate = iconDataTemplate,
                Gesture = gesture,
            });
    }
    
    private T BindTool<T>(T tool) where T : LaminarTool 
    {
        _topLevel.KeyBindings.Add(new KeyBinding
        {
            Command = new ExecuteCommandUnderCursor(tool, this),
            [!KeyBinding.GestureProperty] = tool[!LaminarTool.GestureProperty],
        });

        return tool;
    }
}