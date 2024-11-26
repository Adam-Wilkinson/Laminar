using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Reactive;

namespace Laminar.Avalonia.Commands;

public class LaminarCommandWithParameter : AvaloniaObject
{
    public static readonly StyledProperty<LaminarCommandVisual> CommandVisualProperty =
        AvaloniaProperty.Register<LaminarCommandWithParameter, LaminarCommandVisual>(nameof(CommandVisual));
    
    public static readonly StyledProperty<object?> ParameterProperty = 
        AvaloniaProperty.Register<LaminarCommandWithParameter, object?>(nameof(Parameter));

    public static readonly DirectProperty<LaminarCommandWithParameter, bool> CanExecuteProperty
        = AvaloniaProperty.RegisterDirect<LaminarCommandWithParameter, bool>(
            nameof(CanExecute), 
            o => o.CanExecute);

    private bool _canExecute;
    
    static LaminarCommandWithParameter()
    {
        CommandVisualProperty.Changed.AddClassHandler<LaminarCommandWithParameter>((parameter, args) =>
            parameter.CommandChanged(args));
    }
    
    public LaminarCommandVisual CommandVisual
    {
        get => GetValue(CommandVisualProperty);
        set => SetValue(CommandVisualProperty, value);
    }

    public object? Parameter
    {
        get => GetValue(ParameterProperty);
        set => SetValue(ParameterProperty, value);
    }

    public bool CanExecute
    {
        get => _canExecute;
        set => SetAndRaise(CanExecuteProperty, ref _canExecute, value);
    }
    
    private void CommandChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (Parameter is not INotifyPropertyChanged notifyChangedParameter || CommandVisual?.Command is null) return;
        CommandVisual.Command.CanExecuteChangedFor(notifyChangedParameter).TriggerNotification += (_, _) =>
        {
            CanExecute = CommandVisual.Command.CanExecute(notifyChangedParameter);
        };
        CanExecute = CommandVisual.Command.CanExecute(notifyChangedParameter);
    }
    
    public bool Execute() => CanExecute && CommandVisual.Command.Execute(Parameter);
}