using System.ComponentModel;

namespace Laminar.Contracts.UserData.FileNavigation;

public interface ILaminarStorageItem : INotifyPropertyChanged
{
    public string Name { get; }
    public string Path { get; }
    public bool IsEnabled { get; set; }
    public bool IsEffectivelyEnabled { get; }
    public bool ParentIsEffectivelyEnabled { get; }
    public void Delete();
    public void MoveTo(string newPath);
    public ILaminarStorageFolder ParentFolder { get; }
}