using System;
using System.Collections.Generic;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.Scripting.NodeWrapping;

namespace Laminar.Implementation.Scripting.Actions;

public class DeleteNodeAction(IWrappedNode node, ICollection<IWrappedNode> nodeCollection)
    : IUserAction
{
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute { get; } = nodeCollection.Contains(node);

    public IUserAction Execute()
    {
        nodeCollection.Remove(node);
        return new AddNodeAction(node, nodeCollection);
    }
}
