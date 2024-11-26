using System.ComponentModel;
using Laminar.Domain.Notification;

namespace Laminar.Domain.Extensions;

public static class NotifyPropertyChangedExtensions
{
    private static readonly Dictionary<INotifyPropertyChanged, PropertyChangedFilterHolder> Holders = [];
    
    public static FilteredPropertyChangedEventSource FilterPropertyChanged(this INotifyPropertyChanged notifyPropertyChanged,
        string propertyName)
    {
        if (Holders.TryGetValue(notifyPropertyChanged, out var holder))
        {
            return holder.GetFilter(propertyName);
        }

        var newHolder = new PropertyChangedFilterHolder(notifyPropertyChanged);
        Holders.Add(notifyPropertyChanged, newHolder);
        return newHolder.GetFilter(propertyName);
    }

    private class PropertyChangedFilterHolder
    {
        private readonly Dictionary<string, FilteredPropertyChangedEventSource> _filteredEvents = [];
        
        public PropertyChangedFilterHolder(INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName is not null && _filteredEvents.TryGetValue(args.PropertyName, out var filteredEvent))
                {
                    filteredEvent.InvokePropertyChanged(sender, args);
                } 
            };
        }

        public FilteredPropertyChangedEventSource GetFilter(string propertyName)
        {
            if (_filteredEvents.TryGetValue(propertyName, out var value))
            {
                return value;
            }

            var newFilter = new FilteredPropertyChangedEventSource();
            _filteredEvents.Add(propertyName, newFilter);
            return newFilter;
        }
    }
    
    public class FilteredPropertyChangedEventSource : INotifyPropertyChanged, INotificationSource
    {
        public void InvokePropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(sender, args);
            TriggerNotification?.Invoke(sender, EventArgs.Empty);
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? TriggerNotification;
    }
}