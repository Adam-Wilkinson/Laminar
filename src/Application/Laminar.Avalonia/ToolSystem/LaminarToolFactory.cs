using System;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Domain.Notification;
using Laminar.Domain.ValueObjects;

namespace Laminar.Avalonia.ToolSystem;

public class LaminarToolFactory
{
    private readonly IUserActionManager _userActionManager;
    private readonly TopLevel? _topLevel;
    
    private Visual? _visualUnderCursor;
    private bool _keyboardChordHandled = false;
    
    public LaminarToolFactory(IUserActionManager userActionManager, TopLevel? topLevel)
    {
        _userActionManager = userActionManager;
        _topLevel = topLevel;
        if (_topLevel is null) return;
        _topLevel.PointerMoved += (_, e) =>
        {
            _visualUnderCursor = _topLevel.GetVisualAt(e.GetPosition(_topLevel));
        };
        
        _topLevel.AddHandler(InputElement.KeyUpEvent, (_, e) =>
        {
            if (_keyboardChordHandled) e.Handled = true;
            if (e.KeyModifiers == KeyModifiers.None) _keyboardChordHandled = false;   
        }, RoutingStrategies.Bubble | RoutingStrategies.Tunnel);
    }

    public ToolBuilder DefineTool(string name, IDataTemplate iconTemplate, KeyGesture? keyGesture = null)
        => DefineTool(name, iconTemplate, new ObservableValue<string?>(null), keyGesture);
    
    public ToolBuilder DefineTool(string name, IDataTemplate iconTemplate, string? description, KeyGesture? gesture = null)
        => DefineTool(name, iconTemplate, new ObservableValue<string?>(description), gesture); 
    
    public ToolBuilder DefineTool(string name, IDataTemplate iconTemplate, IObservableValue<string?> description,
        KeyGesture? gesture = null)
        => new(this, name, description, iconTemplate, gesture);

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
        public CommandTool AsCommand(Action action, IObservableValue<bool>? canExecute = null)
            => factory.BindTool(new CommandTool(action, descriptionObservable, canExecute ?? new ObservableValue<bool>(true))
            {
                Name = name,
                IconTemplate = iconDataTemplate,
                Gesture = gesture,
            });

        public CommandTool AsCommand(IUserAction action)
            => factory.BindTool(new CommandTool(() => factory._userActionManager.ExecuteAction(action), descriptionObservable, action.CanExecuteObservable())
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
        public CommandTool<TParameter> AsCommand(Action<TParameter> action,
            ReactiveFunc<TParameter, bool>? canExecute = null)
            => factory.BindTool(new CommandTool<TParameter>(action, descriptionGenerator, canExecute)
            {
                Name = name,
                IconTemplate = iconDataTemplate,
                Gesture = gesture,
            });
        
        public CommandTool<TParameter> AsCommand(IParameterAction<TParameter> action)
            => factory.BindTool(new CommandTool<TParameter>(action, factory._userActionManager, descriptionGenerator)
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
    
    private class ExecuteCommandUnderCursor(ICommand command, LaminarToolFactory factory) : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return TryGetCurrentParameter(out _);
        }

        public void Execute(object? parameter)
        {
            if (!TryGetCurrentParameter(out var currentParameter)) return;
            command.Execute(currentParameter);
            factory._keyboardChordHandled = true;
        }

        public event EventHandler? CanExecuteChanged;

        private bool TryGetCurrentParameter(out object? parameter)
        {
            if (TryGetParameter(factory?._topLevel?.FocusManager?.GetFocusedElement()) is { } focusMatch)
            {
                parameter = focusMatch;
                return true;
            }
            
            if (factory?._visualUnderCursor is null)
            {
                parameter = default;
                return false;
            }

            foreach (var visual in factory._visualUnderCursor.GetVisualAncestors())
            {
                if (TryGetParameter(visual) is not { } underCursorMatch) continue;
                
                parameter = underCursorMatch;
                return true;
            }

            parameter = default;
            return false;
        }

        private object? TryGetParameter(object? target) => target switch
        {
            not null when command.CanExecute(target) => target,
            ContentControl contentControl when command.CanExecute(contentControl.Content) => contentControl.Content,
            ContentPresenter contentPresenter when command.CanExecute(contentPresenter.Content) => contentPresenter.Content,
            _ => null,
        };
    }
}