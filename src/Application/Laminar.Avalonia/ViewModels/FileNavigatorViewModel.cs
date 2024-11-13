using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Laminar.Contracts.UserData;

namespace Laminar.Avalonia.ViewModels;
public class FileNavigatorViewModel : ViewModelBase
{
    private readonly IStorageProvider _storageProvider;
    private readonly ILaminarFileManager _fileManager;
    
    public FileNavigatorViewModel(IStorageProvider storageProvider, ILaminarFileManager fileManager)
    {
        _storageProvider = storageProvider;
        _fileManager = fileManager;

        foreach (var folder in _fileManager.RootFolders)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            
            Files.Add(new Folder(storageProvider.TryGetFolderFromPathAsync(folder).Result!));
        }
    }

    public ObservableCollection<IFileNavigatorItem> Files { get; } = new();

    public void OpenFilePicker()
    {
        _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { AllowMultiple = false, Title = "Pick a file!"});
    }

    public interface IFileNavigatorItem
    {
        public ObservableCollection<IFileNavigatorItem>? Children { get; }
        public string Name { get; }
    }

    public class Folder : IFileNavigatorItem
    {
        public ObservableCollection<IFileNavigatorItem>? Children { get; }
        public string Name { get; }

        public Folder(IStorageFolder folder)
        {
            Name = folder.Name;
            Children = new ObservableCollection<IFileNavigatorItem>();
            foreach (var item in folder.GetItemsAsync().ToBlockingEnumerable())
            {
                Children.Add(item switch
                {
                    IStorageFolder folderItem => new Folder(folderItem),
                    IStorageFile fileItem => new File(fileItem),
                });
            }
        }
    }

    public class File : IFileNavigatorItem
    {
        public ObservableCollection<IFileNavigatorItem>? Children { get; } = null;
        public string Name { get; }

        public File(IStorageFile file)
        {
            Name = file.Name;
        }
    }
}