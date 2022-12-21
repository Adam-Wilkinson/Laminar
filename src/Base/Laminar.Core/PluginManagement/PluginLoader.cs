﻿using Laminar.PluginFramework.Registration;
using Laminar_PluginFramework.NodeSystem.Nodes;
using Laminar_PluginFramework.Registration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Laminar.Core.PluginManagement;

public class PluginLoader
{
    private readonly string[] AutoLoadPlugins = { "Basic Functionality UI", "Base plugin functionality", "Keyboard and Mouse interface", "Windows Base" };
    readonly FrontendDependency _frontend;
    private readonly IPluginHostFactory _pluginHostFactory;

    public PluginLoader(string path, FrontendDependency frontend, IPluginHostFactory pluginHostFactory)
    {
        _frontend = frontend;
        _pluginHostFactory = pluginHostFactory;
        List<IRegisteredPlugin> registeredPlugins = new();
        foreach (string pluginPath in InbuiltPluginFinder.GetInbuiltPlugins(path))
        {
            PluginLoadContext pluginContext = new(pluginPath);
            Assembly pluginAssembly = pluginContext.LoadFromAssemblyPath(pluginPath);
            foreach (Module module in pluginAssembly.Modules)
            {
                if (LoadModule(module) is RegisteredPlugin plugin)
                {
                    registeredPlugins.Add(plugin);
                }
                
            }
        }

        RegisteredPlugins = registeredPlugins.ToArray();
    }

    private RegisteredPlugin? LoadModule(Module module)
    {
        if (module.GetCustomAttribute<HasFrontendDependencyAttribute>() is HasFrontendDependencyAttribute attr && attr.FrontendDependency != _frontend)
        {
            return null;
        }

        foreach (Type type in module.GetTypes())
        {
            if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface)
            {
                IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                RegisteredPlugin registeredPlugin = new(plugin, _pluginHostFactory);
                if (AutoLoadPlugins.Contains(registeredPlugin.PluginName))
                {
                    registeredPlugin.Load();
                }
                return registeredPlugin;
            }
        }

        return null;
    }

    public IRegisteredPlugin[] RegisteredPlugins { get; }

    public class RegisteredPlugin : IRegisteredPlugin
    {
        readonly IPluginHost _host;
        readonly IPlugin _plugin;
        readonly Dictionary<string, Type> _registeredNodesByName = new();
        readonly HashSet<Type> _registeredNodes = new();

        public RegisteredPlugin(IPlugin plugin, IPluginHostFactory pluginHostFactory)
        {
            _host = pluginHostFactory.GetPluginhost(this);
            _plugin = plugin;
            PluginName = plugin.PluginName;
            PluginDescription = plugin.PluginDescription;
        }

        public string PluginName { get; }

        public string PluginDescription { get; }

        public void RegisterNode(INode node)
        {
            _registeredNodes.Add(node.GetType());
            _registeredNodesByName.Add(node.NodeName, node.GetType());
        }

        public bool ContainsNode(INode node)
        {
            return _registeredNodes.Contains(node.GetType());
        }

        public IReadOnlyDictionary<string, Type> RegisteredNodes => _registeredNodesByName;

        public void Load()
        {
            _plugin.Register(_host);
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
