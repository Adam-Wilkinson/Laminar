using System.Collections.ObjectModel;
using System.IO;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;

namespace Laminar.Implementation.UserData;

public class LaminarFileManager(IPersistentDataManager dataManager) : ILaminarFileManager
{
    private readonly IPersistentDataManager _dataManager = dataManager;

    private readonly IPersistentDataStore _rootFoldersDataStore =
        dataManager.GetDataStore(CommonDataStoreKeys.PersistentData, PersistentDataType.Json);
    
    public ObservableCollection<ILaminarRootFolder> RootFolders { get; } = 
    [
        new LaminarRootFolder(Path.Combine(dataManager.Path, "Files")),
    ];
}