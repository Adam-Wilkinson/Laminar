using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Reactive;
using Avalonia.VisualTree;
using Laminar.Avalonia.InitializationTargets;

namespace Laminar.Avalonia.ToolSystem;

public class ToolKeyBindManager(TopLevel topLevel) : IAfterApplicationBuiltTarget
{
    private readonly TopLevel _topLevel = topLevel;
    
    private Visual? _visualUnderCursor;
    private bool _keyChordHandled = false;
    
    public void OnApplicationBuilt()
    {
        _topLevel.PointerMoved += (_, args) =>
        {
            _visualUnderCursor = _topLevel.GetVisualAt(args.GetPosition(_topLevel));
        };

        _topLevel.Resources.GetResourceObservable(ToolTemplate.ToolRootKey).Subscribe(new AnonymousObserver<object?>(
            toolRoot =>
            {
                if (toolRoot is ToolTemplate rootToolTemplate) BindTool(rootToolTemplate);
            }));
        
        // Hack to stop Avalonia from focusing the menu when keybindings involving the Alt key are released.
        // If the bug is fixed, remove this.
        _topLevel.AddHandler(InputElement.KeyUpEvent, (_, e) =>
        {
            if (_keyChordHandled) e.Handled = true;
            if (e.KeyModifiers == KeyModifiers.None) _keyChordHandled = false;
        }, RoutingStrategies.Bubble | RoutingStrategies.Tunnel);
    }

    private void BindTool(ToolTemplate tool)
    {
        _topLevel.KeyBindings.Add(new KeyBinding
        {
            [!KeyBinding.GestureProperty] = tool[!ToolTemplate.GestureProperty],
            Command = new ExecuteToolAtCursor(tool, this),
        });

        foreach (var childTool in tool.ChildTools)
        {
            BindTool(childTool);
        }
    }

    private class ExecuteToolAtCursor(ToolTemplate tool, ToolKeyBindManager keyBindManager) : ICommand
    {
        private ToolInstance? _toolInstanceCache;
        
        public bool CanExecute(object? parameter) 
            => TryBuildTool(out _toolInstanceCache) 
               && _toolInstanceCache?.Command.CanExecute(parameter) == true;

        public void Execute(object? parameter)
        {
            if (_toolInstanceCache is not null && _toolInstanceCache.Command.CanExecute(parameter))
            {
                _toolInstanceCache.Command.Execute(parameter);
                keyBindManager._keyChordHandled = true;
                return;
            }

            if (TryBuildTool(out var toolInstance)
                && toolInstance is not null
                && toolInstance.Command.CanExecute(parameter))
            {
                toolInstance.Command.Execute(parameter);
                keyBindManager._keyChordHandled = true;
            }
        }

        public event EventHandler? CanExecuteChanged;

        private bool TryBuildTool(out ToolInstance? instance)
        {
            if (TryBuildToolFromTarget(keyBindManager._topLevel.FocusManager?.GetFocusedElement()) is { } focusedElementTool)
            {
                instance = focusedElementTool;
                return true;
            }

            if (keyBindManager._visualUnderCursor is null)
            {
                instance = null;
                return false;
            }
            
            foreach (var parentVisual in keyBindManager._visualUnderCursor.GetVisualAncestors())
            {
                if (TryBuildToolFromTarget(parentVisual) is { } hoveredVisualTool)
                {
                    instance = hoveredVisualTool;
                    return true;
                }
            }
            
            instance = null;
            return false;
        }

        private ToolInstance? TryBuildToolFromTarget(object? target) => target switch
        {
            not null when tool.Build(target) is { } instance => instance,
            ContentPresenter contentPresenter when tool.Build(contentPresenter.Content) is { } instance => instance,
            ContentControl contentControl when tool.Build(contentControl.Content) is { } instance => instance,
            StyledElement styledElement when tool.Build(styledElement.DataContext) is { } instance => instance,
            _ => null,
        };
    }
}