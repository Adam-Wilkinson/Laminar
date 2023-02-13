﻿using System.Collections.Generic;
using System.ComponentModel;
using Laminar.Contracts.Primitives;
using Laminar.PluginFramework.NodeSystem;
using Laminar.PluginFramework.UserInterface;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace Laminar.Implementation.Scripting.NodeIO;

internal class DisplayValue<T> : IDisplayValue
{
    private object? _lastValue;
    private readonly INotificationClient _valueChangedByuserNotificationClient;
    private readonly IValueInterfaceDefinition _valueInterfaceDefinition;

    private T _internalValue;

    public DisplayValue(INotificationClient valueChangedByUserNotificationClient, IValueInterfaceDefinition valueInterfaceDefinition, T initialValue)
    {
        _valueChangedByuserNotificationClient = valueChangedByUserNotificationClient;
        _valueInterfaceDefinition = valueInterfaceDefinition;
        _internalValue = initialValue;
    }

    public IValueProvider<T>? ValueProvider { get; set; }

    public required string Name { get; init; }

    public T TypedValue
    {
        get => ValueProvider is null ? _internalValue : ValueProvider.Value;
        set => _internalValue = value;
    }

    public IUserInterfaceDefinition? InterfaceDefinition => _valueInterfaceDefinition.GetCurrentDefinition();

    public object? Value 
    { 
        get => TypedValue;
        set
        {
            if (value is T typedValue && ValueProvider is null && !EqualityComparer<T>.Default.Equals(typedValue, _internalValue))
            {
                TypedValue = typedValue;
                PropertyChanged?.Invoke(this, IDisplayValue.ValueChangedEventArgs);
                _valueChangedByuserNotificationClient.TriggerNotification();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Refresh()
    {
        if (Value != _lastValue)
        {
            PropertyChanged?.Invoke(this, IDisplayValue.ValueChangedEventArgs);
            _lastValue = Value;
        }
    }
}
