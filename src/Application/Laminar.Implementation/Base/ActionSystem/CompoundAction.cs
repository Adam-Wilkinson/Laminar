﻿using System.Collections.Generic;
using System.Linq;
using Laminar.Contracts.Base.ActionSystem;

namespace Laminar.Implementation.Base.ActionSystem;

public class CompoundAction : IUserAction
{
    private readonly List<IUserAction> _actions;

    public CompoundAction(params IUserAction[] actions)
    {
        _actions = actions.ToList();
    }

    public void Add(IUserAction action)
    {
        _actions.Add(action);
    }

    public bool Execute()
    {
        foreach (var userAction in _actions)
        {
            userAction.Execute();
        }

        return true;
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
}
