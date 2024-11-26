namespace Laminar.Domain.DataManagement.FileNavigation;

public class LaminarStorageFile : LaminarStorageItem<FileInfo>
{
    public LaminarStorageFile(FileInfo fileSystemInfo) : base(fileSystemInfo)
    {
    }

    public LaminarStorageFile(string path) : this(new FileInfo(path))
    {
    }

    internal void ParentEnabledChanged(bool enabled)
    {
        ParentIsEffectivelyEnabled = enabled;
        OnPropertyChanged(nameof(IsEffectivelyEnabled));
    }
}