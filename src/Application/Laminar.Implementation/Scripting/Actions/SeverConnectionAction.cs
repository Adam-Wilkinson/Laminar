using System;
using System.Collections.Generic;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.Scripting.Connection;
using Laminar.Domain.ValueObjects;

namespace Laminar.Implementation.Scripting.Actions;

public class SeverConnectionAction(IConnection? connection, ICollection<IConnection> connectionCollection)
    : IUserAction
{
    public IObservableValue<bool> CanExecute { get; } = new ObservableValue<bool>(connection is not null);

    public void Execute()
    {
        ArgumentNullException.ThrowIfNull(connection);
        connection.InputConnector.OnDisconnectedFrom(connection.OutputConnector);
        connection.OutputConnector.OnDisconnectedFrom(connection.InputConnector);
        connectionCollection.Remove(connection);
        connection.Break();
    }

    public IUserAction GetInverse()
    {
        ArgumentNullException.ThrowIfNull(connection);
        return new EstablishConnectionAction(connection.OutputConnector, connection.InputConnector, connectionCollection);
    }
}
