using Laminar.Domain.DataManagement;

namespace Laminar.Contracts.UserData;

public interface IPersistentDataTranscoder
{
    public string FileExtension { get; }

    public string Encode(Dictionary<string, IPersistentDataValue> toEncode);
    
    public Dictionary<string, IPersistentDataValue> Decode(string data, Dictionary<string, IPersistentDataValue> typeHints);
}