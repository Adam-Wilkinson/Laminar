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

    public IObservableValue<bool> CanExecute { get; } =
        new ObservableValue<bool>(connectorOne.CanConnectTo(connectorTwo) || connectorTwo.CanConnectTo(connectorOne));

    void IUserAction.Execute()
    {
        if (!connectorOne.TryConnectTo(connectorTwo) && !connectorTwo.TryConnectTo(connectorOne)) return;
        
        _connection = new Connection
        {
            OutputConnector = connectorOne,
            InputConnector = connectorTwo
        };
        connectionCollection.Add(_connection);
    }

    public IUserAction GetInverse()
    {
        return new SeverConnectionAction(_connection, connectionCollection);
    }
}
