namespace Laminar.Domain.Notification;

public interface INotificationSource
{
    public event EventHandler TriggerNotification;
}