using System;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData.FileNavigation;
using Laminar.Domain.Notification;
using Laminar.Domain.ValueObjects;

namespace Laminar.Implementation.UserData.FileNavigation.UserActions;

public class ToggleEnabledParameterAction : IParameterAction<ILaminarStorageItem>
{
    private static readonly ReactiveFunc<ILaminarStorageItem, bool> CanExecuteReactiveFunc = new(item => item.ParentIsEffectivelyEnabled);
    
    public IObservableValue<bool> CanExecute(ILaminarStorageItem parameter) => CanExecuteReactiveFunc.GetObservable(parameter);

    public IUserAction Execute(ILaminarStorageItem parameter)
    {
        parameter.IsEnabled = !parameter.IsEnabled;
        return new ToggleEnabledAction(parameter, CanExecuteReactiveFunc.GetObservable(parameter));
    }

    public class ToggleEnabledAction : IUserAction
    {
        private readonly ILaminarStorageItem _parameter;
        private readonly IObservableValue<bool> _canExecuteObservable;
        
        public ToggleEnabledAction(ILaminarStorageItem parameter, IObservableValue<bool> canExecuteObservable)
        {
            _parameter = parameter;
            _canExecuteObservable = canExecuteObservable;
            canExecuteObservable.ValueChanged += (_, __) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? CanExecuteChanged;
        public bool CanExecute => _canExecuteObservable.Value;
        public IUserAction Execute()
        {
            _parameter.IsEnabled = !_parameter.IsEnabled;
            return this;
        }
    }
}