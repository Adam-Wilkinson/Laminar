using System;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData.FileNavigation;

namespace Laminar.Implementation.UserData.FileNavigation.UserActions;

public class AddDefaultStorageItemAction<T>(ILaminarStorageFolder parentFolder, ILaminarStorageItemFactory factory) : IUserAction
    where T : class, ILaminarStorageItem
{
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute => true;
    public IUserAction Execute()
    {
        var newItem = factory.AddDefaultToFolder<T>(parentFolder);
        return new DeleteStorageItemAction<T>(newItem);
    }
}