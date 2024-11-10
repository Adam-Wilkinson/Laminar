namespace Laminar.Domain.DataManagement;

public enum DataIoStatus
{
    Success = 0,
    FileBusy = 1,
    SerializerNotRegistered = 3,
    DataNotFound = 404,
    
    UnknownError = 10,
}