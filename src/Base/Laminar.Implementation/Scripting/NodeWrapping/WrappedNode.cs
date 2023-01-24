﻿using System;
using System.Collections.ObjectModel;
using Laminar.Contracts.Primitives;
using Laminar.Contracts.Scripting.NodeWrapping;
using Laminar.Domain.Notification;
using Laminar.Domain.ValueObjects;
using Laminar.Implementation.Scripting.Execution;
using Laminar.PluginFramework.NodeSystem;

namespace Laminar.Implementation.Scripting.Nodes;

public class WrappedNode<T> : IWrappedNode where T : INode, new()
{
    readonly T _coreNode;
    readonly INotificationClient<LaminarExecutionContext>? _userChangedValueNotificationClient;
    readonly INodeFactory _factory;
    readonly Action? _preEvaluateAction;

    public WrappedNode(IWrappedNodeRow nameRow, INodeRowCollectionFactory rowCollectionFactory, INotificationClient<LaminarExecutionContext>? userChangedValueNotificationClient, INodeFactory nodeFactory, T node)
    {
        _factory = nodeFactory;
        _coreNode = node;
        Rows = rowCollectionFactory.CreateNodeRowsForObject(_coreNode, this);
        _userChangedValueNotificationClient = userChangedValueNotificationClient;

        NameRow = nameRow;

        foreach (var field in Rows)
        {
            if (field.OutputConnector is not null && field.OutputConnector.NodeIOConnector.PreEvaluateAction is Action currentActionO)
            {
                _preEvaluateAction += currentActionO;
            }

            if (field.InputConnector?.NodeIOConnector.PreEvaluateAction is Action currentActionI)
            {
                _preEvaluateAction += currentActionI;
            }
        }
    }

    public IWrappedNodeRow NameRow { get; }

    public IReadOnlyObservableCollection<IWrappedNodeRow> Rows { get; set; }

    public ObservableValue<Point> Location { get; set; } = new ObservableValue<Point>(new Point { X = 0, Y = 0 });

    public Identifier<IWrappedNode> Id { get; } = Identifier<IWrappedNode>.New();

    public IWrappedNode Clone(INotificationClient<LaminarExecutionContext>? userChangedValueNotificationClient)
    {
        return _factory.CloneNode<T>(this, userChangedValueNotificationClient);
    }

    public void TriggerNotification(LaminarExecutionContext context)
    {
        if (_userChangedValueNotificationClient is null)
        {
            Update(context);
        }
        else
        {
            _userChangedValueNotificationClient.TriggerNotification(context);
        }
    }

    public void Update(LaminarExecutionContext context)
    {
        if (_preEvaluateAction is not null)
        {
            _preEvaluateAction();
        }

        _coreNode.Evaluate();

        if (context.ExecutionFlags.HasUIUpdateFlag())
        {
            foreach (IWrappedNodeRow field in Rows)
            {
                field.RefreshDisplay();
            }
        }
    }
}
