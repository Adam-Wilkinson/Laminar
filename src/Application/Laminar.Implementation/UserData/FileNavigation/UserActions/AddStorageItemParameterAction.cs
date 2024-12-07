using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData.FileNavigation;
using Laminar.Domain.ValueObjects;

namespace Laminar.Implementation.UserData.FileNavigation.UserActions;

public class AddStorageItemParameterAction<T>(ILaminarStorageItemFactory factory) : IParameterAction<ILaminarStorageFolder>
    where T : class, ILaminarStorageItem
{
    public IObservableValue<bool> CanExecute(ILaminarStorageFolder parameter) => new ObservableValue<bool>(true);
    
    public IUserAction Execute(ILaminarStorageFolder parameter)
    {
        var newItem = factory.AddDefaultToFolder<T>(parameter);
        parameter.Contents.Add(newItem);
        return new DeleteStorageItemAction<T>(newItem);
    }
}