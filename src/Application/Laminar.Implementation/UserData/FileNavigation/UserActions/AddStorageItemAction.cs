using System;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData.FileNavigation;

namespace Laminar.Implementation.UserData.FileNavigation.UserActions;

public class AddStorageItemAction<T>(T item, ILaminarStorageFolder folder) : IUserAction where T : class, ILaminarStorageItem
{
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute => true;
    public IUserAction Execute()
    {
        folder.AddItem(item);
        return new DeleteStorageItemAction<T>(item);
    }
}