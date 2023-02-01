﻿using Laminar.Implementation.Scripting.Connections;
using Laminar.PluginFramework.NodeSystem.Contracts.IO;
using Laminar.PluginFramework.Registration;

namespace Laminar.Implementation.Base.PluginLoading;

public static class StaticRegistrations
{
    public static void Register(IPluginHost host)
    {
        host.RegisterInputConnector<IValueInput, ValueInputConnector>();
        host.RegisterOutputConnector<IValueOutput, ValueOutputConnector>();
    }
}