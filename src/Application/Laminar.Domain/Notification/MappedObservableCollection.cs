using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.VisualBasic;

namespace Laminar.Domain.Notification;

public class MappedObservableCollection<TIn, TOut> : IReadOnlyObservableCollection<TOut>
{
    private readonly List<TOut> _outputItems;
    private readonly IReadOnlyObservableCollection<TIn> _collection;
    private readonly IReadOnlyObservableCollection<TIn> _lastInputItems;

    public MappedObservableCollection(IReadOnlyObservableCollection<TIn> collection, Func<TIn, TOut> map)
    {
        _collection = collection;
        _outputItems = new List<TOut>(collection.Select(map));
        _lastInputItems = collection;
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
                        IList myOldItems = _outputItems.Take(new Range(args.OldStartingIndex, args.OldItems!.Count)).ToList();
                        _outputItems.RemoveRange(args.OldStartingIndex, args.OldItems!.Count);
                        CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, myOldItems, args.OldStartingIndex));   
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        };
    }

    public IEnumerator<TOut> GetEnumerator() => _outputItems.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _outputItems.GetEnumerator();

    public int Count => _collection.Count;

    public TOut this[int index] => _outputItems[index];

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}