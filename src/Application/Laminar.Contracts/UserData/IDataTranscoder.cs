using Laminar.Domain.DataManagement;

namespace Laminar.Contracts.UserData;

public interface IDataTranscoder
{
    public string FileExtension { get; }

    public string Encode<T>(T objectToSave);
    
    public T? Decode<T>(string data);
}