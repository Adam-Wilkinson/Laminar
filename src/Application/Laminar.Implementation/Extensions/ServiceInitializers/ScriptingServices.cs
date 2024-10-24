﻿using Laminar.Contracts.Scripting;
using Laminar.Contracts.Scripting.Connection;
using Laminar.Contracts.Scripting.Execution;
using Laminar.Contracts.Scripting.NodeWrapping;
using Laminar.Implementation.Scripting;
using Laminar.Implementation.Scripting.Connections;
using Laminar.Implementation.Scripting.Execution;
using Laminar.Implementation.Scripting.NodeComponents;
using Laminar.Implementation.Scripting.NodeIO;
using Laminar.Implementation.Scripting.Nodes;
using Laminar.Implementation.Scripting.NodeWrapping;
using Laminar.PluginFramework.NodeSystem.Components;
using Laminar.PluginFramework.NodeSystem.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Implementation.Extensions.ServiceInitializers;

internal static class ScriptingServices
{
    public static IServiceCollection AddScriptingServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<INodeFactory, NodeFactory>();
        serviceCollection.AddSingleton<INodeComponentFactory, NodeComponentFactory>();
        serviceCollection.AddSingleton<INodeIOFactory, NodeIOFactory>();

        serviceCollection.AddSingleton<IScriptEditor, ScriptEditor>();
        serviceCollection.AddSingleton<IScriptFactory, ScriptFactory>();
        serviceCollection.AddSingleton<IScriptExecutionManager, ScriptExecutionManager>();
        serviceCollection.AddSingleton<IExecutionOrderFinder, ExecutionOrderFinder>();

        serviceCollection.AddSingleton<IConnectionBridger, DefaultConnectionBridger>();

        serviceCollection.AddSingleton<ILoadedNodeManager, LoadedNodeManager>();

        return serviceCollection;
    }
}
