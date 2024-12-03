namespace Laminar.Contracts.Base.ActionSystem;

public interface IUserActionManager
{
    public bool ExecuteAction(IUserAction action);
    
    public void Undo();

    public void Redo();
}
