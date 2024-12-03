using System.IO;
using Laminar.Contracts.UserData.FileNavigation;

namespace Laminar.Implementation.UserData.FileNavigation;

public class LaminarStorageFile : LaminarStorageItem<FileInfo>
{
    public LaminarStorageFile(string path, ILaminarStorageFolder parent) : this(new FileInfo(path), parent)
    {
    }

    public LaminarStorageFile(FileInfo fileSystemInfo, ILaminarStorageFolder parent) : base(fileSystemInfo)
    {
        if (!fileSystemInfo.Exists)
        {
            fileSystemInfo.Create();
        }

        ParentFolder = parent;
    }

    public override void MoveTo(string newPath)
    {
        FileSystemInfo.MoveTo(newPath);
    }

    public override ILaminarStorageFolder ParentFolder { get; }

    internal void ParentEnabledChanged(bool enabled)
    {
        ParentIsEffectivelyEnabled = enabled;
        OnPropertyChanged(nameof(IsEffectivelyEnabled));
    }
}