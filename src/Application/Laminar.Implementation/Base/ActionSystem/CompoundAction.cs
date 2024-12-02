using System.Collections.Generic;
using System.Linq;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Domain.ValueObjects;

namespace Laminar.Implementation.Base.ActionSystem;

public class CompoundAction : IUserAction
{
    private readonly List<IUserAction> _actions;
    private readonly ObservableValue<bool> _canExecuteObservable = new(true);
    
    public CompoundAction(params IUserAction[] actions)
    {
        _actions = actions.ToList();
        foreach (var action in _actions)
        {
            action.CanExecute.ValueChanged += ChildCanExecuteChanged;
        }
    }

    public void Add(IUserAction action)
    {
        _actions.Add(action);
        action.CanExecute.ValueChanged += ChildCanExecuteChanged;
    }

    public IObservableValue<bool> CanExecute => _canExecuteObservable;

    public void Execute()
    {
        foreach (var userAction in _actions)
        {
            userAction.Execute();
        }
    }

    public IUserAction GetInverse()
    {
        var inverseList = new IUserAction[_actions.Count];
        for (var i = 0; i < _actions.Count; i++)
        {
            inverseList[_actions.Count - 1 - i] = _actions[i].GetInverse();
        }

        return new CompoundAction(inverseList);
    }

    private void ChildCanExecuteChanged(object? sender, bool value)
    {
        _canExecuteObservable.Value = _actions.All(x => x.CanExecute.Value);
    }
}
