namespace Laminar.Contracts.UserData.FileNavigation;

public interface ILaminarStorageItemFactory
{
    public ILaminarStorageItem FromPath(string path, ILaminarStorageFolder? parentFolder = null);

    public ILaminarStorageItem FromFileSystemInfo(FileSystemInfo fileSystemInfo, ILaminarStorageFolder? parentFolder = null);
    
    public T FromPath<T>(string path, ILaminarStorageFolder? parentFolder = null) where T : class, ILaminarStorageItem;
    
    public T AddDefaultToFolder<T>(ILaminarStorageFolder folder) where T : class, ILaminarStorageItem;
}