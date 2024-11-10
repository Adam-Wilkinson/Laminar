using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;

namespace Laminar.Implementation.UserData;

public class LaminarFileManager(IPersistentDataManager dataManager) : ILaminarFileManager
{
    private readonly IPersistentDataStore _rootFoldersDataStore = dataManager.GetDataStore(DataStoreKey.PersistentData)
        .InitializeDefaultValue("LaminarFileRoots", new List<string> { Path.Combine(dataManager.Path, "Default File Location") });
    
    public ObservableCollection<ILaminarRootFolder> RootFolders { get; } = 
    [
        new LaminarRootFolder(Path.Combine(dataManager.Path, "Files")),
    ];
}