using System;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Domain.Notification;
using Laminar.Domain.ValueObjects;

namespace Laminar.Avalonia.ToolSystem;

public class CommandTool(Action commandAction, IObservableValue<string?> descriptionObservable, IObservableValue<bool>? canExecuteObservable) : LaminarTool(descriptionObservable)
{
    public override IObservableValue<bool> CanExecute(object? _) =>
        canExecuteObservable ?? new ObservableValue<bool>(false);

    public override void Execute(object? parameter)
    {
        commandAction.Invoke();
    }
}

public class CommandTool<TParameter> : LaminarTool<TParameter>
{
    private readonly Action<TParameter> _action;
    private readonly Func<TParameter, IObservableValue<bool>> _canExecuteReactiveFunc;
    
    public CommandTool(IParameterAction<TParameter> action,
        IUserActionManager actionManager,
        ReactiveFunc<TParameter, string?> descriptionTemplate) : base(descriptionTemplate)
    {
        _action = parameter => actionManager.ExecuteAction(action.WithParameter(parameter));
        _canExecuteReactiveFunc = action.CanExecute;
    }

    public CommandTool(Action<TParameter> action, ReactiveFunc<TParameter, string?> descriptionTemplate,
        ReactiveFunc<TParameter, bool>? canExecute = null) : base(descriptionTemplate)
    {
        _action = action;
        _canExecuteReactiveFunc = parameter =>
            canExecute is null ? new ObservableValue<bool>(true) : canExecute.GetObservable(parameter);
    }

    public override IObservableValue<bool> CanExecute(object? parameter) => parameter is TParameter typed 
        ? _canExecuteReactiveFunc(typed)
        : new ObservableValue<bool>(false);

    public override void Execute(object? parameter)
    {
        if (parameter is TParameter typed)
        {
            _action(typed);
        }
    }
}