using Laminar.Domain.DataManagement;

namespace Laminar.Domain.Exceptions;

public class UnknownDataTypeException : Exception
{
    public UnknownDataTypeException(PersistentDataType dataType) : base($"No saving type implemented for persistent data type {dataType}")
    {
    }
}
