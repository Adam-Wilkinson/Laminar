using System;
using System.Collections.Generic;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.Scripting.Connection;
using Laminar.Domain.ValueObjects;

namespace Laminar.Implementation.Scripting.Actions;

public class SeverConnectionAction(IConnection? connection, ICollection<IConnection> connectionCollection)
    : IUserAction
{
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute { get; } = connection is not null;

    public IUserAction Execute()
    {
        ArgumentNullException.ThrowIfNull(connection);
        connection.InputConnector.OnDisconnectedFrom(connection.OutputConnector);
        connection.OutputConnector.OnDisconnectedFrom(connection.InputConnector);
        connectionCollection.Remove(connection);
        connection.Break();
        return new EstablishConnectionAction(connection.OutputConnector, connection.InputConnector,
            connectionCollection);
    }

    public IUserAction GetInverse()
    {
        ArgumentNullException.ThrowIfNull(connection);
        return new EstablishConnectionAction(connection.OutputConnector, connection.InputConnector, connectionCollection);
    }
}
