using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Laminar.Domain.ValueObjects;

namespace Laminar.Avalonia.Commands;

public class LaminarToolInstance : AvaloniaObject
{
    public static readonly StyledProperty<LaminarTool?> ToolProperty =
        AvaloniaProperty.Register<LaminarToolInstance, LaminarTool?>(nameof(Tool));

    public static readonly StyledProperty<object?> ParameterProperty =
        AvaloniaProperty.Register<LaminarToolInstance, object?>(nameof(Parameter));

    public static readonly DirectProperty<LaminarToolInstance, bool> CanExecuteProperty = 
        AvaloniaProperty.RegisterDirect<LaminarToolInstance, bool>(nameof(CanExecute), o => o.CanExecute);
    
    public static readonly DirectProperty<LaminarToolInstance, string?> DescriptionProperty = 
        AvaloniaProperty.RegisterDirect<LaminarToolInstance, string?>(nameof(Description), o => o.Description);

    public static readonly DirectProperty<LaminarToolInstance, IEnumerable<LaminarToolInstance>?>
        ChildToolsProperty =
            AvaloniaProperty.RegisterDirect<LaminarToolInstance, IEnumerable<LaminarToolInstance>?>(
                nameof(ChildTools), o => o.ChildTools);
    
    private IObservableValue<string?>? _descriptionObservable;
    private IObservableValue<bool>? _canExecuteObservable;
    private bool _canExecute;
    private string? _description;
    private IEnumerable<LaminarToolInstance>? _childTools;

    static LaminarToolInstance()
    {
        ToolProperty.Changed.AddClassHandler<LaminarToolInstance>((o, _) => o.Update());
        ParameterProperty.Changed.AddClassHandler<LaminarToolInstance>((o, _) => o.Update());
    }
    
    public LaminarTool? Tool
    {
        get => GetValue(ToolProperty);
        set => SetValue(ToolProperty, value);
    }

    public object? Parameter
    {
        get => GetValue(ParameterProperty);
        set => SetValue(ParameterProperty, value);
    }

    public string? Description
    {
        get => _description;
        set => SetAndRaise(DescriptionProperty, ref _description, value);
    }

    public bool CanExecute
    {
        get => _canExecute;
        set => SetAndRaise(CanExecuteProperty, ref _canExecute, value);
    }

    public IEnumerable<LaminarToolInstance>? ChildTools
    {
        get => _childTools;
        set => SetAndRaise(ChildToolsProperty, ref _childTools, value);
    }
    
    public void Execute()
    {
        if (CanExecute && Tool is not null) Tool.Execute(Parameter);
    }
    
    private void Update()
    {
        IObservableValue<string?>.TransferObservable(ref _descriptionObservable, Tool?.GetDescription(Parameter), SetDescription);
        IObservableValue<bool>.TransferObservable(ref _canExecuteObservable, Tool?.CanExecute(Parameter), SetCanExecute);
        ChildTools = Tool?.ChildTools?.Select(x => new LaminarToolInstance
        {
            Tool = x,
            Parameter = Parameter
        });
    }

    private void SetDescription(object? sender, string? description)
    {
        Description = description;
    }

    private void SetCanExecute(object? sender, bool canExecute)
    {
        CanExecute = canExecute;
    }
}