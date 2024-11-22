namespace Laminar.Domain.DataManagement.FileNavigation;

public abstract class LaminarStorageItem
{
    private bool _parentIsEffectivelyEnabled;
    protected DirectoryInfo DirectoryInfo;

    protected LaminarStorageItem(DirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
        Name = DirectoryInfo.Name;
        Path = DirectoryInfo.FullName;
    }
    
    protected LaminarStorageItem(string path) : this(new DirectoryInfo(path))
    {
    }
    
    public string Path { get; }

    public string Name { get; }

    public virtual bool IsEnabled { get; set; }

    public bool IsEffectivelyEnabled => IsEnabled && _parentIsEffectivelyEnabled;

    public override bool Equals(object? obj)
    {
        return obj is LaminarStorageItem storageItem && storageItem.Path == Path;
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode();
    }

    internal virtual void ParentEnabledChange(bool newValue)
    {
        _parentIsEffectivelyEnabled = newValue;
    }
}