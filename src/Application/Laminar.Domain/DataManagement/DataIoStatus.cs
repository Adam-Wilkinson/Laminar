namespace Laminar.Domain.DataManagement;

public enum DataIoStatus
{
    Success = 0,
    FileBusy = 1,
    FileDoesNotExist = 2,
    SerializerNotRegistered = 3,
    
    UnknownError = 10,
}