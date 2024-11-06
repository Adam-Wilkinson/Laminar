using System.Collections.ObjectModel;

namespace Laminar.Contracts.UserData;

public interface ILaminarFileManager
{
    public ObservableCollection<ILaminarRootFolder> RootFolders { get; }
}