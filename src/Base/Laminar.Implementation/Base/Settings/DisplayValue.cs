﻿using System;
using System.ComponentModel;
using Laminar.PluginFramework.UserInterface;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace Laminar.Implementation.Base.Settings;

internal class DisplayValue<T> : IDisplayValue
{
    IValueInterfaceDefinition _interfaceDefinition;

    public DisplayValue(string name, T value)
    {
        InterfaceDefinition = null;
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public object? Value { get; set; }

    public IUserInterfaceDefinition InterfaceDefinition { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Refresh() => PropertyChanged?.Invoke(this, IDisplayValue.ValueChangedEventArgs);
}
