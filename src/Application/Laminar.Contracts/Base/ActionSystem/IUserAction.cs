using Laminar.Domain.ValueObjects;

namespace Laminar.Contracts.Base.ActionSystem;

public interface IUserAction
{
    public IObservableValue<bool> CanExecute { get; }

    public void Execute();

    public IUserAction GetInverse();
}
