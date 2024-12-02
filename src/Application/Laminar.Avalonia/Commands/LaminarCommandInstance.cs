using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Laminar.Domain.ValueObjects;

namespace Laminar.Avalonia.Commands;

public class LaminarCommandInstance : AvaloniaObject
{
    public static readonly StyledProperty<LaminarCommand?> CommandProperty =
        AvaloniaProperty.Register<LaminarCommandInstance, LaminarCommand?>(nameof(Command));

    public static readonly StyledProperty<object?> ParameterProperty =
        AvaloniaProperty.Register<LaminarCommandInstance, object?>(nameof(Parameter));

    public static readonly DirectProperty<LaminarCommandInstance, bool> CanExecuteProperty = 
        AvaloniaProperty.RegisterDirect<LaminarCommandInstance, bool>("CanExecute", o => o.CanExecute);
    
    public static readonly DirectProperty<LaminarCommandInstance, string> DescriptionProperty = 
        AvaloniaProperty.RegisterDirect<LaminarCommandInstance, string>("Description", o => o.Description);

    public static readonly DirectProperty<LaminarCommandInstance, IEnumerable<LaminarCommandInstance>?>
        ChildCommandsProperty =
            AvaloniaProperty.RegisterDirect<LaminarCommandInstance, IEnumerable<LaminarCommandInstance>?>(
                "ChildCommands", o => o.ChildCommands);
    
    private IObservableValue<string>? _descriptionObservable;
    private IObservableValue<bool>? _canExecuteObservable;
    private bool _canExecute;
    private string _description = "";
    private IEnumerable<LaminarCommandInstance>? _childCommands;

    static LaminarCommandInstance()
    {
        CommandProperty.Changed.AddClassHandler<LaminarCommandInstance>((o, _) => o.Update());
        ParameterProperty.Changed.AddClassHandler<LaminarCommandInstance>((o, _) => o.Update());
    }
    
    public LaminarCommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? Parameter
    {
        get => GetValue(ParameterProperty);
        set => SetValue(ParameterProperty, value);
    }

    public string Description
    {
        get => _description;
        set => SetAndRaise(DescriptionProperty, ref _description, value);
    }

    public bool CanExecute
    {
        get => _canExecute;
        set => SetAndRaise(CanExecuteProperty, ref _canExecute, value);
    }

    public IEnumerable<LaminarCommandInstance>? ChildCommands
    {
        get => _childCommands;
        set => SetAndRaise(ChildCommandsProperty, ref _childCommands, value);
    }
    
    public void Execute()
    {
        if (CanExecute && Command is not null) Command.Execute(Parameter);
    }
    
    private void Update()
    {
        IObservableValue<string>.TransferObservable(ref _descriptionObservable, Command?.GetDescription(Parameter), SetDescription);
        IObservableValue<bool>.TransferObservable(ref _canExecuteObservable, Command?.CanExecute(Parameter), SetCanExecute);
        ChildCommands = Command?.ChildCommands?.Select(x => new LaminarCommandInstance
        {
            Command = x,
            Parameter = Parameter
        });
    }

    private void SetDescription(object? sender, string? description)
    {
        Description = description ?? "";
    }

    private void SetCanExecute(object? sender, bool canExecute)
    {
        CanExecute = canExecute;
    }
}