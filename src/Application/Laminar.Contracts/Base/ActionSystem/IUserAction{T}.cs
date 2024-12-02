using Laminar.Domain.ValueObjects;

namespace Laminar.Contracts.Base.ActionSystem;

public static class ParameterActionExtensions
{
    public static IUserAction WithParameter<T>(this IParameterAction<T> parameterAction, T parameter)
        => new ActionWithParameter<T>(parameterAction, parameter);
    
    private class ActionWithParameter<T>(IParameterAction<T> action, T parameter) : IUserAction
    {
        public IObservableValue<bool> CanExecute { get; } = action.CanExecute(parameter);
        
        public void Execute()
            => action.Execute(parameter);

        public IUserAction GetInverse()
            => new ActionWithParameter<T>(action.GetInverse(), parameter);
    }
}

public interface IParameterAction<in T>
{
    public IObservableValue<bool> CanExecute(T parameter);
    
    public void Execute(T parameter);

    public IParameterAction<T> GetInverse();
}