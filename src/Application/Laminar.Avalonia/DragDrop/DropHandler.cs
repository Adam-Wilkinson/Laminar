using System;
using Avalonia;
using Avalonia.Interactivity;

namespace Laminar.Avalonia.DragDrop;

public class DropHandler : Interactive
{
    public static readonly RoutedEvent<DragEventArgs> DropEvent = RoutedEvent.Register<DropHandler, DragEventArgs>(nameof(Drop), RoutingStrategies.Direct);
    
    public static readonly RoutedEvent<DragEventArgs> HoverEnterEvent = RoutedEvent.Register<DropHandler, DragEventArgs>(nameof(HoverEnter), RoutingStrategies.Direct);

    public static readonly RoutedEvent<DragEventArgs> HoverLeaveEvent = RoutedEvent.Register<DropHandler, DragEventArgs>(nameof(HoverLeave), RoutingStrategies.Direct);
    
    public static readonly AttachedProperty<DropAcceptor> DropAcceptorProperty = AvaloniaProperty.RegisterAttached<DropHandler, Visual, DropAcceptor>(nameof(DropAcceptor), defaultValue: new DropAcceptor());
    public static DropAcceptor GetDropAcceptor(Visual visual) => visual.GetValue(DropAcceptorProperty);
    public static void SetDropAcceptor(Visual visual, DropAcceptor value) => visual.SetValue(DropAcceptorProperty, value);

    static DropHandler()
    {
        // DropEvent
    }
    
    public event EventHandler<DragEventArgs> HoverLeave
    {
        add => AddHandler(HoverLeaveEvent, value);
        remove => RemoveHandler(HoverLeaveEvent, value);
    }
    
    public event EventHandler<DragEventArgs> Drop
    {
        add => AddHandler(DropEvent, value);
        remove => RemoveHandler(DropEvent, value);
    }
    
    public event EventHandler<DragEventArgs> HoverEnter
    {
        add => AddHandler(HoverEnterEvent, value);
        remove => RemoveHandler(HoverEnterEvent, value);
    }
}