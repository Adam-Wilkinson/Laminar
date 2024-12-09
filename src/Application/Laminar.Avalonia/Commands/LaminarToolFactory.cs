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

public class LaminarToolFactory
{
    private readonly IUserActionManager _userActionManager;
    private readonly TopLevel? _topLevel;

    public LaminarToolFactory(IUserActionManager userActionManager, TopLevel? topLevel)
    {
        _userActionManager = userActionManager;
        _topLevel = topLevel;
        if (_topLevel is not null)
        {
            _topLevel.PointerMoved += (_, e) =>
            {
                VisualUnderCursor = _topLevel.GetVisualAt(e.GetPosition(_topLevel));
            };   
        }
    }

    public Visual? VisualUnderCursor { get; private set; }

    public ToolBuilder DefineTool(string name, IDataTemplate iconTemplate, string? description  = null, KeyGesture? gesture = null)
        => DefineTool(name, iconTemplate, new ObservableValue<string?>(description), gesture); 
    
    public ToolBuilder DefineTool(string name, IDataTemplate iconTemplate, IObservableValue<string?>? description = null,
        KeyGesture? gesture = null)
        => new(this, name, description ?? new ObservableValue<string?>(null), iconTemplate, gesture);

    public ToolBuilder<T> DefineTool<T>(string name, IDataTemplate iconTemplate, string? description = null,
        KeyGesture? gesture = null)
        => DefineTool(name, iconTemplate, new ReactiveFunc<T, string?>(_ => description), gesture);
    
    public ToolBuilder<T> DefineTool<T>(string name, IDataTemplate iconTemplate, Func<T, string?> autoDescriptionGenerator, KeyGesture? gesture = null)
        => DefineTool(name, iconTemplate, new ReactiveFunc<T, string?>(autoDescriptionGenerator), gesture);
    
    public ToolBuilder<T> DefineTool<T>(string name, IDataTemplate iconTemplate, ReactiveFunc<T, string?> descriptionGenerator, KeyGesture? gesture = null) 
        => new(this, name, descriptionGenerator, iconTemplate, gesture);

    public class ToolBuilder(
        LaminarToolFactory factory,
        string name,
        IObservableValue<string?> descriptionObservable,
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
        LaminarToolFactory factory,
        string name, 
        ReactiveFunc<TParameter, string?> descriptionGenerator, 
        IDataTemplate iconDataTemplate, 
        KeyGesture? gesture)
    {
        public Command<TParameter> AsCommand(Action<TParameter> action,
            ReactiveFunc<TParameter, bool>? canExecute = null)
            => factory.BindTool(new Command<TParameter>(action, descriptionGenerator, canExecute)
            {
                Name = name,
                IconTemplate = iconDataTemplate,
                Gesture = gesture,
            });
        
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
        _topLevel?.KeyBindings.Add(new KeyBinding
        {
            Command = new ExecuteCommandUnderCursor(tool, this),
            [!KeyBinding.GestureProperty] = tool[!LaminarTool.GestureProperty],
        });

        return tool;
    }
}