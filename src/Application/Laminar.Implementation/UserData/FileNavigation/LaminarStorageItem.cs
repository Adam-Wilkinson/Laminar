using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Laminar.Contracts.UserData.FileNavigation;

namespace Laminar.Implementation.UserData.FileNavigation;

public abstract class LaminarStorageItem<T> : ILaminarStorageItem
    where T : FileSystemInfo
{
    private bool _isEnabled = true;
    private bool _parentIsEffectivelyEnabled = true;

    protected LaminarStorageItem(T fileSystemInfo)
    {
        FileSystemInfo = fileSystemInfo;
        Extension = FileSystemInfo.Extension;
        Name = string.IsNullOrEmpty(Extension) ? FileSystemInfo.Name : FileSystemInfo.Name.Replace(Extension, string.Empty);
        Path = FileSystemInfo.FullName;
    }

    protected T FileSystemInfo { get; }

    public bool ParentIsEffectivelyEnabled
    {
        get => _parentIsEffectivelyEnabled;
        set
        {
            SetField(ref _parentIsEffectivelyEnabled, value);
            OnPropertyChanged(nameof(IsEffectivelyEnabled));
        }
    }

    public string Extension { get; }
    public string Path { get; }

    public string Name { get; }

    public virtual bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            SetField(ref _isEnabled, value);
            OnPropertyChanged(nameof(IsEffectivelyEnabled));
        }
    }

    public bool IsEffectivelyEnabled => IsEnabled && ParentIsEffectivelyEnabled;

    public override bool Equals(object? obj)
    {
        return obj is LaminarStorageItem<T> storageItem && storageItem.Path == Path;
    }

    public override int GetHashCode()
    {
        return Path.GetHashCode();
    }

    public void Delete()
    {
        FileSystemInfo.Delete();
    }

    public abstract void MoveTo(string newPath);

    public abstract ILaminarStorageFolder ParentFolder { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<TField>(ref TField field, TField value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<TField>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}