using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Platform.Storage;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement.FileNavigation;

namespace Laminar.Avalonia.ViewModels;
public class FileNavigatorViewModel(IStorageProvider storageProvider, IPersistentDataManager dataManager) : ViewModelBase
{
    private readonly IStorageProvider _storageProvider = storageProvider;

    [Serialize]
    public ObservableCollection<LaminarStorageFolder> RootFiles { get; set; } = [ new(Path.Combine(dataManager.Path, "Default")) ];

    public void OpenFilePicker()
    {
        _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { AllowMultiple = false, Title = "Pick a file!"});
    }
}