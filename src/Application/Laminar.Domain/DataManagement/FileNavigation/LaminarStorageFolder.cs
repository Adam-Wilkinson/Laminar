using System.Collections.ObjectModel;

namespace Laminar.Domain.DataManagement.FileNavigation;

public class LaminarStorageFolder : LaminarStorageItem<DirectoryInfo>
{
    public LaminarStorageFolder(string path) : this(new DirectoryInfo(path))
    {
    }
    
    public LaminarStorageFolder(DirectoryInfo directoryInfo) : base(directoryInfo)
    {
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        Contents = new ObservableCollection<ILaminarStorageItem>(directoryInfo.GetDirectories()
            .Select(ILaminarStorageItem.FromFileSystemInfo)
            .Concat(directoryInfo.GetFiles().Select(ILaminarStorageItem.FromFileSystemInfo)));
    }
    
    public ObservableCollection<ILaminarStorageItem> Contents { get; }

    public override bool IsEnabled
    {
        get => base.IsEnabled;
        set
        {
            base.IsEnabled = value;
            EffectivelyEnabledChanged();
        }
    }

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