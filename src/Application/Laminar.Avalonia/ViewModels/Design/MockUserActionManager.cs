using Laminar.Contracts.Base.ActionSystem;

namespace Laminar.Avalonia.ViewModels.Design;

public class MockUserActionManager : IUserActionManager
{
    public bool ExecuteAction(IUserAction action)
    {
        if (!action.CanExecute) return false;
        
        action.Execute();
        return true;

    }

    public void Undo()
    {
    }

    public void Redo()
    {
    }
}