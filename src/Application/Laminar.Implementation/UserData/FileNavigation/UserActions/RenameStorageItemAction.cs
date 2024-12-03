using System;
using System.IO;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData.FileNavigation;

namespace Laminar.Implementation.UserData.FileNavigation.UserActions;

public class RenameStorageItemAction(string newName, ILaminarStorageItem item) : IUserAction
{
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute => true;
    public IUserAction Execute()
    {
        var oldName = item.Name;
        item.MoveTo(newName);
        return new RenameStorageItemAction(oldName, item);
    }
}