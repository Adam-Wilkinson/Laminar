using System;
using System.IO;
using Laminar.Contracts.UserData.FileNavigation;

namespace Laminar.Implementation.UserData.FileNavigation;

public class LaminarStorageItemFactory : ILaminarStorageItemFactory
{
    public ILaminarStorageItem FromPath(string path)
        => (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory
            ? new LaminarStorageFolder(path, this)
            : new LaminarStorageFile(path, new LaminarStorageFolder(new DirectoryInfo(path).Parent!, this));

    public ILaminarStorageItem FromFileSystemInfo(FileSystemInfo fileSystemInfo, ILaminarStorageFolder? parent = null) => fileSystemInfo switch
    {
        DirectoryInfo dir => new LaminarStorageFolder(dir, this, parent),
        FileInfo file => new LaminarStorageFile(file, parent ?? new LaminarStorageFolder(file.Directory!, this)),
        _ => throw new ArgumentException($"Unknown file system type {fileSystemInfo.GetType()}", nameof(fileSystemInfo)),
    };

    public T FromPath<T>(string path, ILaminarStorageFolder? parent = null) where T : class, ILaminarStorageItem
    {
        parent ??= new LaminarStorageFolder(new DirectoryInfo(path).Parent!, this); 
        
        if (typeof(T) == typeof(LaminarStorageFolder))
        {
            return (new LaminarStorageFolder(path, this, parent) as T)!;
        }

        if (typeof(T) == typeof(LaminarStorageFile))
        {
            return (new LaminarStorageFile(path, parent) as T)!;
        }
        
        throw new ArgumentException($"Unknown file system type {typeof(T)}", nameof(path));
    }
}