using System;
using System.Collections.Generic;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.Scripting.Connection;
using Laminar.Domain.ValueObjects;
using Laminar.Implementation.Scripting.Connections;
using Laminar.PluginFramework.NodeSystem.Connectors;

namespace Laminar.Implementation.Scripting.Actions;

public class EstablishConnectionAction(
    IOutputConnector connectorOne,
    IInputConnector connectorTwo,
    ICollection<IConnection> connectionCollection)
    : IUserAction
{
    private IConnection? _connection;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute { get; } = connectorOne.CanConnectTo(connectorTwo) || connectorTwo.CanConnectTo(connectorOne);

    public IUserAction Execute()
    {
        if (!connectorOne.TryConnectTo(connectorTwo) && !connectorTwo.TryConnectTo(connectorOne)) return IUserAction.Pass;
        
        _connection = new Connection
        {
            OutputConnector = connectorOne,
            InputConnector = connectorTwo
        };
        connectionCollection.Add(_connection);

        return new SeverConnectionAction(_connection, connectionCollection);
    }

    public IUserAction GetInverse()
    {
        return new SeverConnectionAction(_connection, connectionCollection);
    }
}
