using System.IO;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;
using Newtonsoft.Json;

namespace Laminar.Implementation.UserData;

public class JsonFileSaver : IFileSaver
{
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        // TypeNameHandling = TypeNameHandling.All,
        // TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
    };

    public DataSaveResult Save<T>(string filePath, T objectToSave)
    {
        var json = JsonConvert.SerializeObject(objectToSave, Formatting.Indented, JsonSettings);
        
        using var stream = File.CreateText(filePath + ".json");
        stream.Write(json);
        return new DataSaveResult(DataIoStatus.Success);
    }

    public DataReadResult<T> Load<T>(string filePath)
    {
        if (!File.Exists(filePath + ".json"))
        {
            File.Create(filePath + ".json").Close();
            return new DataReadResult<T>(default);
        }
        
        var json = File.ReadAllText(filePath + ".json");
        
        if (JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }) is not { } fromJson)
        {
            return new DataReadResult<T>(default, DataIoStatus.UnknownError);
        }

        return new DataReadResult<T>(fromJson);
    }
}