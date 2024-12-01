using System;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Domain.Notification;
using Laminar.Domain.ValueObjects;

namespace Laminar.Avalonia.Commands;

public class ParameterCommand<T>(
    IUserActionManager actionManager,
    string name,
    Func<T, bool> executeFunc,
    Func<T, bool>? undoFunc,
    ReactiveFunc<T, bool> canExecute,
    ReactiveFunc<T, string> description)
    : LaminarCommand(actionManager, name, TypeCheck(executeFunc), TypeCheck(undoFunc))
{
    public override IObservableValue<string> GetDescription(object? parameter)
        => parameter is T correctType
            ? description.GetObservable(correctType)
            : new ObservableValue<string>("Command parameter error");

    public override IObservableValue<bool> CanExecute(object? parameter)
        => parameter is T correctType ? canExecute.GetObservable(correctType) : new ObservableValue<bool>(false);
    
    private static Func<object?, TOutput?> TypeCheck<TInput, TOutput>(Func<TInput, TOutput>? typedAction)
    {
        return obj => obj is TInput typedObject && typedAction is not null ? typedAction(typedObject) : default;
    }
}