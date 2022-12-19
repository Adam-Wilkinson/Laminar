﻿namespace Laminar.Domain;

public interface IObservableCollection<T> : IReadOnlyObservableCollection<T>
{
    public void Add(T item);

    public bool Remove(T item);
}
