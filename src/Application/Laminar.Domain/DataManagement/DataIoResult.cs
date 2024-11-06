namespace Laminar.Domain.DataManagement;

public readonly record struct DataSaveResult(DataIoStatus Status);

public readonly record struct DataReadResult<T>(T? Result, DataIoStatus Status);