namespace Laminar.Domain.DataManagement;

public readonly record struct DataSaveResult(DataIoStatus Status = DataIoStatus.Success, Exception? Exception = null);

public readonly record struct DataReadResult<T>(T? Result, DataIoStatus Status = DataIoStatus.Success, Exception? Exception = null);