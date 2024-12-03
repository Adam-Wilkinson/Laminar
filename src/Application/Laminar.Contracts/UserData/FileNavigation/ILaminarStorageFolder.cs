namespace Laminar.Contracts.UserData.FileNavigation;

public interface ILaminarStorageFolder : ILaminarStorageItem
{
    public T AddItem<T>(T item) where T : class, ILaminarStorageItem;
}