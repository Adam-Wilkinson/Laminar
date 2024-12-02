using System.ComponentModel;

namespace Laminar.Domain.ValueObjects;

public interface IObservableValue<T> : INotifyPropertyChanged
{
    public T Value { get; }

    public event EventHandler<T>? ValueChanged;

    public static void TransferObservable(ref IObservableValue<T>? observableValue,
        IObservableValue<T>? newObservableValue, EventHandler<T> valueChangedHandler)
    {
        if (observableValue is not null)
        {
            observableValue.ValueChanged -= valueChangedHandler;
        }
        
        observableValue = newObservableValue;

        if (observableValue is not null)
        {
            observableValue.ValueChanged += valueChangedHandler;
            valueChangedHandler.Invoke(observableValue, observableValue.Value);
        }
    }
}