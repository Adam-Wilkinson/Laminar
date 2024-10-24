﻿using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Laminar.Contracts.Base.PluginLoading;
using Laminar.Contracts.Scripting;
using Laminar.Implementation.Base.PluginLoading;
using Laminar.Implementation.Extensions.ServiceInitializers;
using Laminar.PluginFramework.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Implementation;

public class Instance
{
    public Instance(SynchronizationContext? uiContext, FrontendDependency supportedDependencies, [CallerFilePath] string path = "")
    {
        PluginFramework.LaminarFactory.ServiceProvider = ServiceProvider;
        _ = new PluginLoader(path, supportedDependencies, ServiceProvider.GetService<IPluginHostFactory>()!);
        // _isLoading = true;
        //foreach (var serializedScript in UserData.LoadAllFromFolder<ISerializedObject<IAdvancedScript>>("Scripts", "las"))
        //{
        //    AllAdvancedScripts.Add(Serializer.Deserialize(serializedScript, null));
        //}
        // _isLoading = false;
    }

    public ObservableCollection<IScript> AllScripts { get; } = new();

    public IServiceProvider ServiceProvider { get; } = new ServiceCollection()
        .AddScriptingServices()
        .AddUserInterfaceServices()
        .AddPluginServices()
        .AddEnvironmentServices()
        .BuildServiceProvider();
}
