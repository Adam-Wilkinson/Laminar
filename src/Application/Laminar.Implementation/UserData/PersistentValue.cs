using System;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData;

public class PersistentValue
{
    private readonly ISerializer _serializer;
    
    private object _defaultValue;
    private object _value;
    
    public PersistentValue(ISerializer serializer, object defaultValue)
    {
        _serializer = serializer;
        _value = defaultValue;
        _defaultValue = defaultValue;
        SerializedValue = _serializer.TrySerializeObject(_value);
        SerializedType = SerializedValue.GetType();
        ValueType = defaultValue.GetType();
    }
    
    public Type ValueType { get; }

    public Type SerializedType { get; }

    public object Value
    {
        get => _value;
        set
        {
            _value = value;
            SerializedValue = _serializer.TrySerializeObject(_value);
        }
    }

    public object SerializedValue { get; private set; }

    public void ResetToDefault()
    {
        Value = _defaultValue;
    }
}