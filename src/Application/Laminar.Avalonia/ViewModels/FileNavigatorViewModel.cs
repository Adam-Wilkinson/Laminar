using System.Collections.ObjectModel;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Laminar.Avalonia.ViewModels;

public class FileNavigatorViewModel(IStorageProvider storageProvider) : ViewModelBase
{
    private readonly IStorageProvider _storageProvider = storageProvider;
    
    public FileNavigatorViewModel() : this(null) { }
    
    public ObservableCollection<TreeTester> Files { get; } =
    [
        new("Root File", [
            new("Sub File One"), 
            new("Sub File Two", [
                new("Sub Sub File One"),
                new("Sub Sub File Two")
            ])
        ])
    ];

    public void OpenFilePicker()
    {
        _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { AllowMultiple = false, Title = "Pick a file!"});
    }
    
    public class TreeTester
    {
        public ObservableCollection<TreeTester>? SubNodes { get; }
        public string Title { get; }

        public TreeTester(string title)
        {
            Title = title;
        }

        public TreeTester(string title, ObservableCollection<TreeTester> subNodes)
            : this(title)
        {
            SubNodes = subNodes;
        }
    }
}