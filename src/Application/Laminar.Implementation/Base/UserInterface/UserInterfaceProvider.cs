using System;
using Laminar.Contracts.Base.UserInterface;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace Laminar.Implementation.Base.UserInterface;

public class UserInterfaceProvider(IReadOnlyUserInterfaceStore interfaceStore) : IUserInterfaceProvider
{
    public object GetUserInterface(IUserInterfaceDefinition? definition)
    {
        if (definition is not null 
            && interfaceStore.TryGetUserInterface(definition.GetType(), out var userInterface)
            && Activator.CreateInstance(userInterface) is { } obj)
        {
            return obj;
        }

        if (interfaceStore.TryGetUserInterface(typeof(DefaultViewer), out var defaultViewer)
            && Activator.CreateInstance(defaultViewer) is { } defaultObj)
        {
            return defaultObj;
        }

        throw new Exception("Interface implementation asked for but not found");
    }

    public bool InterfaceImplemented(IUserInterfaceDefinition interfaceDefinition)
    {
        return interfaceStore.HasImplementation(interfaceDefinition.GetType());
    }
}