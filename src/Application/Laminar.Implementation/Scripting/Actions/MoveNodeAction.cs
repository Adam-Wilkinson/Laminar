using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.Scripting.NodeWrapping;
using Laminar.Domain.ValueObjects;

namespace Laminar.Implementation.Scripting.Actions;

public class MoveNodeAction(IWrappedNode items, Point locationDelta) : IUserAction
{
    public IObservableValue<bool> CanExecute { get; } = new ObservableValue<bool>(true);

    public void Execute() => items.Location.Value += locationDelta;

    public IUserAction GetInverse()
    {
        return new MoveNodeAction(items, -locationDelta);
    }
}