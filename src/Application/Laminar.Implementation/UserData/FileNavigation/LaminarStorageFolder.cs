using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Laminar.Contracts.UserData.FileNavigation;

namespace Laminar.Implementation.UserData.FileNavigation;

public class LaminarStorageFolder : LaminarStorageItem<DirectoryInfo>, ILaminarStorageFolder
{
    private readonly ILaminarStorageItemFactory _factory;
    
    private ILaminarStorageFolder? _parent;
    
    public LaminarStorageFolder(string path, ILaminarStorageItemFactory factory, ILaminarStorageFolder? parent = null) : this(new DirectoryInfo(path), factory, parent)
    {
    }
    
    public LaminarStorageFolder(DirectoryInfo directoryInfo, ILaminarStorageItemFactory factory, ILaminarStorageFolder? parent = null) : base(directoryInfo)
    {
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        _factory = factory;
        
        Contents = new ObservableCollection<ILaminarStorageItem>(GetChildren());

        _parent = parent;
    }
    
    public ObservableCollection<ILaminarStorageItem> Contents { get; }

    public override ILaminarStorageFolder ParentFolder => _parent ??= new LaminarStorageFolder(FileSystemInfo.Parent!, _factory);
    
    public override bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            base.IsEnabled = value;
            EffectivelyEnabledChanged();
        }
    }

    public override void MoveTo(string newPath)
    {
        FileSystemInfo.MoveTo(newPath);
    }

    public T AddItem<T>(T item) where T : class, ILaminarStorageItem
    {
        FileSystemInfo.Refresh();
        
        Contents.Clear();
        foreach (var child in GetChildren())
        {
            Contents.Add(child);
        }
        
        return item;
    }

    private IEnumerable<ILaminarStorageItem> GetChildren() => FileSystemInfo.GetDirectories()
        .Select(x => _factory.FromFileSystemInfo(x, this))
        .Concat(FileSystemInfo.GetFiles().Select(x => _factory.FromFileSystemInfo(x, this)));
    
    private void ParentEnabledChange(bool newValue)
    {
        ParentIsEffectivelyEnabled = newValue;
        EffectivelyEnabledChanged();
    }

    private void EffectivelyEnabledChanged()
    {
        OnPropertyChanged(nameof(IsEffectivelyEnabled));
        foreach (var storageItem in Contents)
        {
            switch (storageItem)
            {
                case LaminarStorageFile file:
                    file.ParentEnabledChanged(IsEffectivelyEnabled);
                    break;
                case LaminarStorageFolder folder:
                    folder.ParentEnabledChange(IsEffectivelyEnabled);
                    break;
            }
        }
    }
}