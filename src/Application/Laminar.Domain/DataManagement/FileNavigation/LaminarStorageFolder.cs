using System.Collections.ObjectModel;

namespace Laminar.Domain.DataManagement.FileNavigation;

public class LaminarStorageFolder : LaminarStorageItem
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

        Contents = new ObservableCollection<LaminarStorageItem>(directoryInfo.GetDirectories()
            .Select(x => new LaminarStorageFolder(x)));
    }
    
    public ObservableCollection<LaminarStorageItem> Contents { get; }
    
    internal override void ParentEnabledChange(bool newValue)
    {
        base.ParentEnabledChange(newValue);
        foreach (var storageItem in Contents)
        {
            storageItem.ParentEnabledChange(newValue);
        }
    }
}