﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using Laminar.Avalonia.NodeDisplaySystem;
using Laminar.Contracts.Scripting.Connection;
using Laminar.Domain.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Avalonia.Controls.ScriptEditor;

internal class ConnectionCanvas : Canvas
{
    public static readonly StyledProperty<IReadOnlyObservableCollection<IConnection>> ConnectionsProperty = AvaloniaProperty.Register<ConnectionCanvas, IReadOnlyObservableCollection<IConnection>>(nameof(Connections));

    private readonly Dictionary<IConnection, IDisposable> _inputConnectionObservables = new();
    private readonly Dictionary<IConnection, IDisposable> _outputConnectionObservables = new();
    private readonly Dictionary<IConnection, ConnectionGeometry> _connections = new();
    private readonly INotifyCollectionChangedHelper _notificationHelper = App.LaminarInstance.ServiceProvider.GetService<INotifyCollectionChangedHelper>();

    public IReadOnlyObservableCollection<IConnection> Connections
    {
        get => GetValue(ConnectionsProperty);
        set
        {
            if (Connections is not null)
            {
                _notificationHelper.HelperInstance(Connections).ItemAdded -= ConnectionManager_ConnectionEstablished;
                _notificationHelper.HelperInstance(Connections).ItemRemoved -= ConnectionManager_ConnectionLost;
                _connections.Clear();
            }

            SetValue(ConnectionsProperty, value);
            if (Connections is not null)
            {
                _notificationHelper.HelperInstance(Connections).ItemAdded += ConnectionManager_ConnectionEstablished;
                _notificationHelper.HelperInstance(Connections).ItemRemoved += ConnectionManager_ConnectionLost;
                foreach (IConnection connection in Connections)
                {
                    AddConnection(connection);
                }
            }

            InvalidateVisual();
        }
    }

    public IEnumerable<ConnectionGeometry> ConnectionGeometries => _connections.Values;

    public override void Render(DrawingContext context)
    {
        foreach (ConnectionGeometry connection in _connections.Values)
        {
            if (connection.GetValue(Selection.SelectedProperty))
            {
                context.DrawGeometry(null, new Pen(Brushes.White, connection.Pen.Thickness * 1.7), connection);
            }

            context.DrawGeometry(null, connection.Pen, connection);
        }
        base.Render(context);
    }

    public void DeleteSelection(SelectionManager selectionManager)
    {
        foreach (ConnectionGeometry connection in selectionManager.GetSelection<ConnectionGeometry>())
        {
            connection.CoreConnection.Break();
        }
    }

    private void ConnectionManager_ConnectionLost(object sender, ItemRemovedEventArgs<IConnection> e)
    {
        _connections[e.Item].Changed -= ConnectionGeometryChanged;
        _connections.Remove(e.Item);
        _inputConnectionObservables[e.Item].Dispose();
        _inputConnectionObservables.Remove(e.Item);
        _outputConnectionObservables[e.Item].Dispose();
        _outputConnectionObservables.Remove(e.Item);
        InvalidateVisual();
    }

    private void ConnectionManager_ConnectionEstablished(object sender, ItemAddedEventArgs<IConnection> e) => AddConnection(e.Item);

    private void AddConnection(IConnection newConnection)
    {
        ConnectionGeometry newConnectionGeometry = new() { CoreConnection = newConnection, Pen = new Pen(new SolidColorBrush(Color.Parse(newConnection.OutputConnector.ColorHex)), 3) };
        _connections.Add(newConnection, newConnectionGeometry);


        newConnectionGeometry[!ConnectionGeometry.EndPointProperty] = ConnectorControl.FromConnector(newConnection.InputConnector).GetObservable(TransformedBoundsProperty).Select(GetTransformBoundsCenter).ToBinding();
        _inputConnectionObservables.Add(newConnection, ConnectorControl.FromConnectorObservable(newConnection.InputConnector).Subscribe(connectorControl =>
        {
            newConnectionGeometry[!ConnectionGeometry.EndPointProperty] = connectorControl.GetObservable(TransformedBoundsProperty).Select(GetTransformBoundsCenter).ToBinding();
        }));

        newConnectionGeometry[!ConnectionGeometry.StartPointProperty] = ConnectorControl.FromConnector(newConnection.OutputConnector).GetObservable(TransformedBoundsProperty).Select(GetTransformBoundsCenter).ToBinding();
        _outputConnectionObservables.Add(newConnection, ConnectorControl.FromConnectorObservable(newConnection.OutputConnector).Subscribe(connectorControl =>
        {
            newConnectionGeometry[!ConnectionGeometry.StartPointProperty] = connectorControl.GetObservable(TransformedBoundsProperty).Select(GetTransformBoundsCenter).ToBinding();
        }));

        newConnectionGeometry.Changed += ConnectionGeometryChanged;

        InvalidateVisual();
    }

    private void ConnectionGeometryChanged(object sender, EventArgs e) => InvalidateVisual();

    private Point? GetTransformBoundsCenter(TransformedBounds? rect)
        => rect?.Bounds.Center.Transform(rect.Value.Transform * this.TransformedBounds.Value.Transform.Invert());
}