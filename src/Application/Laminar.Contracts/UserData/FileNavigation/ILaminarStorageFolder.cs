using System.Collections.ObjectModel;

namespace Laminar.Contracts.UserData.FileNavigation;

public interface ILaminarStorageFolder : ILaminarStorageItem
{
    public ObservableCollection<ILaminarStorageItem> Contents { get; }
}