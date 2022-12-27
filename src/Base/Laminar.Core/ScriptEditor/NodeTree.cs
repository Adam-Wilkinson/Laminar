﻿using System;
using System.Collections.Generic;
using Laminar.Contracts.NodeSystem;
using Laminar.Contracts.NodeSystem.Connection;
using Laminar.PluginFramework.NodeSystem.Contracts.Connectors;

namespace Laminar.Core.ScriptEditor;

public class NodeTree : INodeTree
{
    readonly Dictionary<INodeWrapper, List<INodeWrapper>> _dependentNodes = new();
    readonly Dictionary<INodeWrapper, INodeWrapper[]> _knownExecutionPaths = new();
    readonly Dictionary<IIOConnector, INodeWrapper> _connectorParents = new();

    List<INodeWrapper>? _currentExecutionTree;
    IReadOnlyList<INodeWrapper> _currentExecutionLevel;

    public NodeTree(IScript script)
    {
        script.Nodes.ItemAdded += NodeAdded;
        script.Nodes.ItemRemoved += NodeRemoved;

        script.Connections.ItemAdded += ConnectionAdded;
        script.Connections.ItemRemoved += ConnectionRemoved;
    }

    public IReadOnlyList<INodeWrapper> GetDirectDependents(INodeWrapper node) => _dependentNodes.TryGetValue(node, out List<INodeWrapper> dependentNodes) ? dependentNodes : Array.Empty<INodeWrapper>();

    public INodeWrapper[] GetExecutionOrder(INodeWrapper node)
    {
        if (_knownExecutionPaths.TryGetValue(node, out INodeWrapper[] executionOrder))
        {
            return executionOrder;
        }

        List<INodeWrapper> newExecutionOrder = new() { node };
        _currentExecutionLevel = GetDirectDependents(node);
        List<INodeWrapper> nextExecutionLevel = new();

        while (_currentExecutionLevel.Count > 0)
        {
            foreach (var currentNode in _currentExecutionLevel)
            {
                newExecutionOrder.Add(currentNode);
                nextExecutionLevel.AddRange(GetDirectDependents(currentNode));
            }
            _currentExecutionLevel = nextExecutionLevel;
            nextExecutionLevel = new();
        }

        INodeWrapper[] newOrderArray = newExecutionOrder.ToArray();
        _knownExecutionPaths.Add(node, newOrderArray);
        return newOrderArray;
    }

    private void ConnectionRemoved(object sender, IConnection removedConnection)
    {
        INodeWrapper outputNode = _connectorParents[removedConnection.OutputConnector];
        _dependentNodes[outputNode].Remove(_connectorParents[removedConnection.InputConnector]);

        if (_dependentNodes[outputNode].Count == 0)
        {
            _dependentNodes.Remove(outputNode);
        }

        _knownExecutionPaths.Clear();
    }

    private void ConnectionAdded(object sender, IConnection newConnection)
    {
        INodeWrapper outputNode = _connectorParents[newConnection.OutputConnector];
        if (!_dependentNodes.ContainsKey(outputNode))
        {
            _dependentNodes.Add(outputNode, new List<INodeWrapper>());
        }

        _dependentNodes[outputNode].Add(_connectorParents[newConnection.InputConnector]);

        _knownExecutionPaths.Clear();
    }

    private void NodeRemoved(object sender, INodeWrapper removedNode)
    {
        foreach (var field in removedNode.Fields)
        {
            if (field.InputConnector is not null)
            {
                _connectorParents.Remove(field.InputConnector.NodeIOConnector);
            }

            if (field.OutputConnector is not null)
            {
                _connectorParents.Remove(field.OutputConnector.NodeIOConnector);
            }
        }
    }

    private void NodeAdded(object sender, INodeWrapper newNode)
    {
        foreach (var field in newNode.Fields)
        {
            if (field.InputConnector is not null)
            {
                _connectorParents.Add(field.InputConnector.NodeIOConnector, newNode);
            }

            if (field.OutputConnector is not null)
            {
                _connectorParents.Add(field.OutputConnector.NodeIOConnector, newNode);
            }
        }
    }
}
