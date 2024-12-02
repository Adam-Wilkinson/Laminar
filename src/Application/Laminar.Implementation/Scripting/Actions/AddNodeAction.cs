using System.Collections.Generic;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.Base.UserInterface;
using Laminar.Contracts.Scripting.NodeWrapping;
using Laminar.Domain.ValueObjects;
using Laminar.PluginFramework.NodeSystem.Components;

namespace Laminar.Implementation.Scripting.Actions;

public class AddNodeAction(IWrappedNode node, ICollection<IWrappedNode> nodeCollection)
    : IUserAction
{
    public IObservableValue<bool> CanExecute { get; } = new ObservableValue<bool>(true);

    void IUserAction.Execute()
    {
        nodeCollection.Add(node);
    }

    public IUserAction GetInverse()
    {
        return new DeleteNodeAction(node, nodeCollection);
    }
}
