﻿using System;
using Laminar.Contracts.NodeSystem.Connection;
using Laminar.PluginFramework.NodeSystem.Contracts.Connectors;

namespace Laminar.Core.ScriptEditor.Connections;

//internal record class Connection(IOutputConnector OutputConnector, IInputConnector InputConnector) : IConnection
//{
//    public event EventHandler OnBroken;

//    public void Break()
//    {
//        OnBroken?.Invoke(this, EventArgs.Empty);
//    }
//}

internal class Connection : IConnection
{
    public IInputConnector InputConnector { get; init; }

    public IOutputConnector OutputConnector { get; init; }

    public event EventHandler OnBroken;

    public void Break()
    {
        OnBroken?.Invoke(this, EventArgs.Empty);
    }
}