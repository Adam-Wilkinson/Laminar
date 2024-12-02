using System.Collections.Generic;
using Laminar.Contracts.Base.ActionSystem;

namespace Laminar.Implementation.Base.ActionSystem;

internal class UserActionManager : IUserActionManager
{
    private readonly List<IUserAction> _undoList = [];
    private readonly List<IUserAction> _redoList = [];

    private int _compoundNestDepth = 0;
    private CompoundAction? _currentCompoundAction;

    public bool ExecuteAction(IUserAction action)
    {
        if (!action.CanExecute.Value) return false;
        
        action.Execute();
        
        if (_compoundNestDepth > 0 && _currentCompoundAction is not null)
        {
            _currentCompoundAction.Add(action);
        }
        else
        {
            _undoList.Add(action.GetInverse());
        }
        
        return true;
    }

    public void Undo()
    {
        var successfulAction = false;
        while (!successfulAction && _undoList.Count > 0)
        {
            if (_undoList[^1].CanExecute.Value)
            {
                _undoList[^1].Execute();
                _redoList.Add(_undoList[^1].GetInverse());
                successfulAction = true;
            }

            _undoList.RemoveAt(_undoList.Count - 1);
        }
    }

    public void Redo()
    {
        var successfulAction = false;
        while (!successfulAction && _redoList.Count > 0)
        {
            if (_redoList[^1].CanExecute.Value)
            {
                _redoList[^1].Execute();
                _undoList.Add(_redoList[^1].GetInverse());
                successfulAction = true;
            }

            _redoList.RemoveAt(_redoList.Count - 1);
        }
    }

    public void BeginCompoundAction()
    {
        if (_compoundNestDepth == 0)
        {
            _currentCompoundAction = new CompoundAction();
        }

        _compoundNestDepth++;
    }

    public void EndCompoundAction()
    {
        _compoundNestDepth--;
        if (_compoundNestDepth != 0 || _currentCompoundAction is null) return;
        
        _undoList.Add(_currentCompoundAction.GetInverse());
        _currentCompoundAction = null;
    }

    public void ResetCompoundAction()
    {
        _currentCompoundAction?.GetInverse().Execute();
        _currentCompoundAction = new CompoundAction();
    }
}
