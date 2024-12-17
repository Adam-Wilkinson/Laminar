using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.VisualBasic;

namespace Laminar.Domain.Notification;

public class MappedObservableCollection<TIn, TOut> : IReadOnlyObservableCollection<TOut>
{
    private readonly List<TOut> _outputItems;

    public static MappedObservableCollection<TIn, TOut> New<TCollection>(TCollection collection, Func<TIn, TOut> map)
        where TCollection : INotifyCollectionChanged, IEnumerable<TIn>
        => new(collection, map);

    public MappedObservableCollection(IReadOnlyObservableCollection<TIn> collection, Func<TIn, TOut> map) : this(
        (INotifyCollectionChanged)collection, map)
    {
    }
    
    public MappedObservableCollection(ObservableCollection<TIn> collection, Func<TIn, TOut> map) : this((INotifyCollectionChanged)collection, map)
    {
    }
    
    private MappedObservableCollection(INotifyCollectionChanged collection, Func<TIn, TOut> map)
    {
        if (collection is not IEnumerable<TIn> inputEnumerable)
        {
            throw new ArgumentException("Invalid collection type", nameof(collection));
        }
        
        _outputItems = [..inputEnumerable.Select(map)];
        collection.CollectionChanged += (sender, args) =>
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (args.NewItems!.Count == 1)
                    {
                        var newItem = map((TIn)args.NewItems![0]!);
                        _outputItems.Insert(args.NewStartingIndex, newItem);
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, args.NewStartingIndex));   
                    }
                    else
                    {
                        var newItems = args.NewItems!.Cast<TIn>().Select(map).ToList();
                        _outputItems.InsertRange(args.NewStartingIndex, newItems);
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, args.NewStartingIndex));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (args.OldItems!.Count == 1)
                    {
                        var oldItem = _outputItems[args.OldStartingIndex];
                        _outputItems.RemoveAt(args.OldStartingIndex);
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, args.OldStartingIndex));
                    }
                    else
                    {
                        var myOldItems = _outputItems.Take(new Range(args.OldStartingIndex, args.OldStartingIndex + args.OldItems!.Count)).ToList();
                        _outputItems.RemoveRange(args.OldStartingIndex, args.OldItems!.Count);
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, myOldItems, args.OldStartingIndex));   
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (args.OldItems!.Count == 1 && args.NewItems!.Count == 1)
                    {
                        var oldItem = _outputItems[args.OldStartingIndex];
                        var newItem = map((TIn)args.NewItems![0]!);
                        _outputItems[args.OldStartingIndex] = newItem;
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, args.OldStartingIndex));
                    }
                    else
                    {
                        var newItems = args.NewItems!.Cast<TIn>().Select(map).ToList();
                        var oldItems = _outputItems.Take(new Range(args.OldStartingIndex, args.OldStartingIndex + args.OldItems!.Count)).ToList();
                        _outputItems.RemoveRange(args.OldStartingIndex, args.OldItems!.Count);
                        _outputItems.InsertRange(args.NewStartingIndex, newItems);
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems, args.OldStartingIndex));
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (args.OldItems!.Count == 1)
                    {
                        var movedItem = _outputItems[args.OldStartingIndex];
                        _outputItems.RemoveAt(args.OldStartingIndex);
                        _outputItems.Insert(args.NewStartingIndex, movedItem);
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, movedItem, args.NewStartingIndex, args.OldStartingIndex));
                    }
                    else
                    {
                        var movedItems = _outputItems.Take(new Range(args.OldStartingIndex, args.OldStartingIndex + args.OldItems!.Count)).ToList();
                        _outputItems.RemoveRange(args.OldStartingIndex, args.OldItems!.Count);
                        _outputItems.InsertRange(args.NewStartingIndex, movedItems);
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, movedItems, args.NewStartingIndex, args.OldStartingIndex));
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _outputItems.Clear();
                    CollectionChanged?.Invoke(sender, args);
                    break;
            }
        };
    }

    public IEnumerator<TOut> GetEnumerator() => _outputItems.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _outputItems.GetEnumerator();

    public int Count => _outputItems.Count;

    public TOut this[int index] => _outputItems[index];

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}