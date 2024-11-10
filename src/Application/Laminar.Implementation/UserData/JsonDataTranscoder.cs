using Laminar.Contracts.UserData;
using Newtonsoft.Json;

namespace Laminar.Implementation.UserData;

public class JsonDataTranscoder : IDataTranscoder
{
    private static readonly JsonSerializerSettings JsonSettings = new();

    public string FileExtension => ".json";
    
    public string Encode<T>(T objectToSave) 
        => JsonConvert.SerializeObject(objectToSave, Formatting.Indented, JsonSettings);

    public T? Decode<T>(string data) 
        => JsonConvert.DeserializeObject<T>(data, JsonSettings);
}