using System;
using Laminar.Contracts.UserData;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData;

public class PersistentValue : IPersistentDataValue
{
    private readonly ISerializer _serializer;
    private readonly object _defaultValue;
    
    private object _value;
    private object _serializedValue;
    
    public PersistentValue(ISerializer serializer, object defaultValue)
    {
        _serializer = serializer;
        _value = defaultValue;
        _defaultValue = defaultValue;
        _serializedValue = _serializer.SerializeObject(_value);
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
            if (value.GetType() != ValueType)
            {
                throw new Exception("The new value type is not the same as the persistent data type");
            }
            
            _value = value;
            _serializedValue = _serializer.SerializeObject(_value);
        }
    }

    public object SerializedValue
    {
        get => _serializedValue;
        set
        {
            if (value.GetType() != SerializedType)
            {
                throw new ArgumentException("The value type is not the same as the serialized value type.");
            }
            
            _serializedValue = value;
            _value = _serializer.DeserializeObject(_serializedValue, ValueType);
        }
    }

    public void ResetToDefault()
    {
        Value = _defaultValue;
    }
}