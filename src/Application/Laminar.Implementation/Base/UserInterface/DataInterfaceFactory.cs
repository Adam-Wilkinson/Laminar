using System;
using System.Collections.Generic;
using System.ComponentModel;
using Laminar.Contracts.Base;
using Laminar.Contracts.Base.UserInterface;
using Laminar.PluginFramework.UserInterface;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace Laminar.Implementation.Base.UserInterface;

public class DataInterfaceFactory(ITypeInfoStore typeInfoStore) : IDataInterfaceFactory
{
    private readonly Dictionary<(Type definitionType, Type valueType), Func<IInterfaceData, IInterfaceData?>> _interfaceFactories = [];
    private readonly Dictionary<Type, List<Type>?> _frontendImplementations = [];
    
    public void RegisterInterface<TInterfaceDefinition, TValue, TInterface>()
        where TInterfaceDefinition : IUserInterfaceDefinition, new()
        where TValue : notnull
    {
        _interfaceFactories[(typeof(TInterfaceDefinition), typeof(TValue))] = CreateGenericInterfaceDataFactory<TInterfaceDefinition, TValue>();
         _frontendImplementations[typeof(TInterfaceDefinition)] ??= [];
         _frontendImplementations[typeof(TInterfaceDefinition)]!.Add(typeof(TInterface));
    }

    public IDataInterface<TFrontend> GetDataInterface<TFrontend>(IInterfaceData interfaceData)
        where TFrontend : class, new() => new DataInterface<TFrontend>(interfaceData, this);

    public (TFrontend, IInterfaceData) GetFrontendAndData<TFrontend>(IInterfaceData interfaceData)
        where TFrontend : class, new()
    {
        if (_interfaceFactories.TryGetValue((interfaceData.Definition.GetType(), interfaceData.Value.GetType()), out var genericInterfaceDataFactory) 
            && genericInterfaceDataFactory(interfaceData) is { } genericInterfaceData 
            && GetFrontendFromData<TFrontend>(genericInterfaceData) is { } preferredFrontend)
        {
            return (preferredFrontend, genericInterfaceData);
        }

        if (typeInfoStore.TryGetTypeInfo(interfaceData.Value.GetType(), out var interfaceDataTypeInfo))
        {
            var requestedDefinition = interfaceData.IsUserEditable ? interfaceDataTypeInfo.EditorDefinition : interfaceDataTypeInfo.ViewerDefinition;
            if (requestedDefinition is IUserInterfaceDefinition requestedInterfaceDefinition && _interfaceFactories.TryGetValue((requestedInterfaceDefinition.GetType(), interfaceData.Value.GetType()), out var typeInterfaceDataFactory)
                && typeInterfaceDataFactory(interfaceData) is { } typeInterfaceData
                && GetFrontendFromData<TFrontend>(typeInterfaceData) is { } typeInterfaceFrontend)
            {
                return (typeInterfaceFrontend, typeInterfaceData);
            }
        }
        
        var defaultViewerData = new InterfaceDataGenericWrapper<DefaultViewer, object>(interfaceData);
        if (GetFrontendFromData<TFrontend>(defaultViewerData) is { } defaultFrontend)
        {
            return (defaultFrontend, defaultViewerData);
        }

        throw new Exception();
    }

    private TFrontend? GetFrontendFromData<TFrontend>(IInterfaceData interfaceData) 
        where TFrontend : class, new()
    {
        foreach (var frontendType in _frontendImplementations[interfaceData.Definition.GetType()]!)
        {
            if (typeof(TFrontend).IsAssignableFrom(frontendType) && Activator.CreateInstance(frontendType) is TFrontend frontend)
            {
                return frontend;
            }
        }

        return null;
    }
    
    private static Func<IInterfaceData, IInterfaceData?> CreateGenericInterfaceDataFactory<TInterfaceDefinition, TValue>() 
        where TInterfaceDefinition : IUserInterfaceDefinition, new() where TValue : notnull
        => interfaceData => interfaceData switch {
            IInterfaceData<TInterfaceDefinition, TValue> genericInterfaceData => genericInterfaceData,
            IInterfaceData<TValue> genericValueInterfaceData => new InterfaceDataGenericWrapper<TInterfaceDefinition, TValue>(genericValueInterfaceData),
            not null => new InterfaceDataGenericWrapper<TInterfaceDefinition, TValue>(interfaceData),
            _ => null,
        };
}

public class InterfaceDataGenericWrapper<TInterfaceDefinition, TValue> : IInterfaceData<TInterfaceDefinition, TValue>
    where TInterfaceDefinition : IUserInterfaceDefinition, new()
    where TValue : notnull
{
    private readonly IInterfaceData _internal;
    private readonly IInterfaceData<TValue>? _genericDataInternal;
    
    public InterfaceDataGenericWrapper(IInterfaceData<TValue> interfaceDefinition) : this((IInterfaceData)interfaceDefinition)
    {
        _genericDataInternal = interfaceDefinition;
    }

    public InterfaceDataGenericWrapper(IInterfaceData interfaceDefinition)
    {
        _internal = interfaceDefinition;
        _internal.PropertyChanged += InterfaceDefinition_PropertyChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    public bool IsUserEditable => _internal.IsUserEditable;

    public TValue Value
    {
        get => _genericDataInternal is not null ? _genericDataInternal.Value : (TValue)_internal.Value;
        set
        {
            if (_genericDataInternal is not null) _genericDataInternal.Value = value;
            else _internal.Value = value;
        }
    }

    public string Name => _internal.Name;

    private void InterfaceDefinition_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IInterfaceData.Definition)) return;
        PropertyChanged?.Invoke(this, e);
    }
    
    public TInterfaceDefinition Definition => (TInterfaceDefinition)_internal.Definition;
}