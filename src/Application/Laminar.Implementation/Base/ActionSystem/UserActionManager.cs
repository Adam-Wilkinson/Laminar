using System.Collections.Generic;
using Laminar.Contracts.Base.ActionSystem;

namespace Laminar.Implementation.Base.ActionSystem;

internal class UserActionManager : IUserActionManager
{
    private readonly List<IUserAction> _undoList = new();
    private readonly List<IUserAction> _redoList = new();

    private int _compoundNestDepth = 0;
    private CompoundAction? _currentCompoundAction;

    public bool ExecuteAction(IUserAction action)
    {
        if (!action.Execute()) return false;
        
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
            successfulAction = _undoList[^1].Execute();
            if (successfulAction)
            {
                _redoList.Add(_undoList[^1].GetInverse());
            }

            _undoList.RemoveAt(_undoList.Count - 1);
        }
    }

    public void Redo()
    {
        var successfulAction = false;
        while (!successfulAction && _redoList.Count > 0)
        {
            successfulAction = _redoList[^1].Execute();
            if (successfulAction)
            {
                _undoList.Add(_redoList[^1].GetInverse());
            }

            _redoList.RemoveAt(_redoList.Count - 1);
        }
    }

    public void BeginCompoundAction()
    {
        if (_compoundNestDepth == 0)
        {
            _currentCompoundAction = new();
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

    public void ResetCompountAction()
    {
        _currentCompoundAction?.GetInverse().Execute();
        _currentCompoundAction = new();
    }
}
