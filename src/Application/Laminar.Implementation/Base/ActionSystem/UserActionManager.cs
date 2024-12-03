using System.Collections.Generic;
using Laminar.Contracts.Base.ActionSystem;

namespace Laminar.Implementation.Base.ActionSystem;

internal class UserActionManager : IUserActionManager
{
    private readonly List<IUserAction> _undoList = [];
    private readonly List<IUserAction> _redoList = [];

    public bool ExecuteAction(IUserAction action)
    {
        if (!action.CanExecute) return false;
        _undoList.Add(action.Execute());
        return true;
    }

    public void Undo()
    {
        var successfulAction = false;
        while (!successfulAction && _undoList.Count > 0)
        {
            if (_undoList[^1].CanExecute)
            {
                _redoList.Add(_undoList[^1].Execute());
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
            if (_redoList[^1].CanExecute)
            {
                _undoList.Add(_redoList[^1].Execute());
                successfulAction = true;
            }

            _redoList.RemoveAt(_redoList.Count - 1);
        }
    }
}
