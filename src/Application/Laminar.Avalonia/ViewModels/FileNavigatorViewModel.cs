using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Laminar.Contracts.Base;

namespace Laminar.Avalonia.ViewModels;
public class FileNavigatorViewModel : ViewModelBase
{
    private readonly IStorageProvider _storageProvider;

    public FileNavigatorViewModel(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
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