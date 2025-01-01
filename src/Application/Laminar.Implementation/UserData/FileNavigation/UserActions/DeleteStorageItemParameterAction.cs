using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData.FileNavigation;
using Laminar.Domain.ValueObjects;

namespace Laminar.Implementation.UserData.FileNavigation.UserActions;

public class DeleteStorageItemParameterAction<T>(ILaminarStorageItemFactory factory) : IParameterAction<T> where T : class, ILaminarStorageItem
{
    public IObservableValue<bool> CanExecute(T parameter) => new ObservableValue<bool>(true);

    public IUserAction Execute(T parameter)
        => new DeleteStorageItemAction<T>(parameter).Execute();
}