using System.ComponentModel;

namespace Laminar.Domain.DataManagement.FileNavigation;

public interface ILaminarStorageItem : INotifyPropertyChanged
{
    public string Name { get; }
    public string Path { get; }
    public bool IsEnabled { get; set; }
    public bool IsEffectivelyEnabled { get; }
    public bool ParentIsEffectivelyEnabled { get; }
    public void Delete();
    
    public static ILaminarStorageItem FromPath(string path)
        => (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory ? new LaminarStorageFolder(path) : new LaminarStorageFile(path);

    public static ILaminarStorageItem FromFileSystemInfo(FileSystemInfo fileSystemInfo) => fileSystemInfo switch
    {
        DirectoryInfo dir => new LaminarStorageFolder(dir),
        FileInfo file => new LaminarStorageFile(file),
        _ => throw new ArgumentException($"Unknown file system type {fileSystemInfo.GetType()}", nameof(fileSystemInfo)),
    };
}