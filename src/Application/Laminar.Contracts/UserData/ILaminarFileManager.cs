using System.Collections.ObjectModel;

namespace Laminar.Contracts.UserData;

public interface ILaminarFileManager
{
    public List<string> RootFolders { get; }
}