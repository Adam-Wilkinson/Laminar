namespace Laminar.Contracts.UserData;

public interface ILaminarRootFolder
{
    public string Path { get; }

    public bool IsEnabled { get; set; }
}