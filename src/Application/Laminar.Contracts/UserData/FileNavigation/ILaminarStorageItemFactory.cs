namespace Laminar.Contracts.UserData.FileNavigation;

public interface ILaminarStorageItemFactory
{
    public ILaminarStorageItem FromPath(string path);

    public ILaminarStorageItem FromFileSystemInfo(FileSystemInfo fileSystemInfo, ILaminarStorageFolder? parentFolder = null);
    
    public T FromPath<T>(string path, ILaminarStorageFolder? parentFolder = null) where T : class, ILaminarStorageItem;
}