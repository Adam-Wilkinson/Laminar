using Laminar.Domain.DataManagement;

namespace Laminar.Contracts.UserData;

public interface IFileSaver
{
    public DataSaveResult Save<T>(string filePath, T objectToSave);
    
    public DataReadResult<T> Load<T>(string filePath);
}