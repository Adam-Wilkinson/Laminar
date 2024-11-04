using System;
using System.Collections.Generic;
using Laminar.Contracts.Base;
using Laminar.Domain;

namespace Laminar.Implementation.Base;

public class TypeInfoStore : ITypeInfoStore
{
    private readonly Dictionary<Type, TypeInfo> _typeInfoStore = new();

    public TypeInfo GetTypeInfoOrBlank(Type? type)
        => type is not null && _typeInfoStore.TryGetValue(type, out var typeInfo)
            ? typeInfo
            : new TypeInfo("Unknown Type", null, null, "#FFFFFF", default);

    public bool RegisterType(Type type, TypeInfo typeInfo)
    {
        return _typeInfoStore.TryAdd(type, typeInfo);
    }

    public bool TryGetTypeInfo(Type type, out TypeInfo info) => _typeInfoStore.TryGetValue(type, out info);
}