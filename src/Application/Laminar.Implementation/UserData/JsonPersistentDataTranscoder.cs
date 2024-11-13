using System;
using System.Collections.Generic;
using System.Linq;
using Laminar.Contracts.UserData;
using System.Text.Json;
using System.Text.Json.Serialization;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Implementation.UserData;

public class JsonPersistentDataTranscoder : IPersistentDataTranscoder
{
    private readonly PersistentDataValueConverter _persistentDataConverter = new();

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
    };

    public JsonPersistentDataTranscoder()
    {
        _jsonOptions.Converters.Add(_persistentDataConverter);
    }
    
    public string FileExtension => ".json";
    
    public string Encode(Dictionary<string, IPersistentDataValue> objectToSave) 
        => JsonSerializer.Serialize(objectToSave, _jsonOptions);

    public Dictionary<string, IPersistentDataValue> Decode(string data,
        Dictionary<string, IPersistentDataValue> typeHints)
    {
        _persistentDataConverter.TypeHints = typeHints;
        return JsonSerializer.Deserialize<Dictionary<string, IPersistentDataValue>>(data, _jsonOptions) ?? [];
    } 
    
    private class PersistentDataValueConverter : JsonConverter<Dictionary<string, IPersistentDataValue>>
    {
        public Dictionary<string, IPersistentDataValue> TypeHints { get; set; } = [];
        
        public override Dictionary<string, IPersistentDataValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return TypeHints;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                if (reader.GetString() is not { } propertyName || !TypeHints.TryGetValue(propertyName, out var dataVal))
                {
                    throw new JsonException();
                }
                
                reader.Read();
                if (JsonDocument.ParseValue(ref reader).Deserialize(dataVal.SerializedType, options) is not { } readValue)
                {
                    throw new JsonException();
                }

                dataVal.SerializedValue = readValue;
            }

            return TypeHints;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, IPersistentDataValue> dict, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var (key, value) in dict)
            {
                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(key) ?? key);
                JsonSerializer.SerializeToElement(value.SerializedValue, options).WriteTo(writer);
            }
            
            writer.WriteEndObject();
        }
    }
}