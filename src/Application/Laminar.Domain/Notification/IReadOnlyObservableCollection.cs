using System.Collections.Specialized;

namespace Laminar.Domain.Notification;

public interface IReadOnlyObservableCollection<out T> : IReadOnlyList<T>, INotifyCollectionChanged
{
}
