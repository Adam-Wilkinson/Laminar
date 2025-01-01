using System.Collections.ObjectModel;
using System.IO;
using Laminar.Avalonia.ViewModels.Services;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData;
using Laminar.Contracts.UserData.FileNavigation;

namespace Laminar.Avalonia.ViewModels;
public class FileNavigatorViewModel(
    IUserActionManager actionManager, 
    IPersistentDataManager dataManager, 
    ILaminarStorageItemFactory storageItemFactory)
    : ViewModelBase
{
    [Serialize]
    public ObservableCollection<FileNavigatorItemViewModel> RootFiles { get; set; } = [ 
        new(storageItemFactory.FromPath(Path.Combine(dataManager.Path, "Default")), actionManager, storageItemFactory) 
    ];

    public FileNavigatorItemViewModel NewItem(ILaminarStorageItem coreItem) =>
        new(coreItem, actionManager, storageItemFactory);
    
    public void OpenFilePicker()
    {
    }
}