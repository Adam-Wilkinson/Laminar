﻿using Laminar.PluginFramework.NodeSystem.Contracts;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace Laminar.Contracts.Base.UserInterface;

public interface IUserInterfaceProvider
{
    bool InterfaceImplemented(IUserInterfaceDefinition interfaceDefinition);

    object GetUserInterface(IUserInterfaceDefinition definition);

    IUserInterfaceDefinition FindDefinitionForValueInfo(IValueInfo valueInfo);
}