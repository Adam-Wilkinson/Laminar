using System;
using Avalonia;
using Avalonia.Interactivity;

namespace Laminar.Avalonia.DragDrop;

public class DropHandler : Interactive
{
    public static readonly RoutedEvent<DragEventArgs> DropEvent = RoutedEvent.Register<DropHandler, DragEventArgs>(nameof(Drop), RoutingStrategies.Direct);
    
    public static readonly RoutedEvent<DragEventArgs> HoverEvent = RoutedEvent.Register<DropHandler, DragEventArgs>(nameof(Hover), RoutingStrategies.Direct);

    public static readonly StyledProperty<Predicate<Visual>> AcceptsVisualProperty =
        AvaloniaProperty.Register<DropHandler, Predicate<Visual>>(nameof(AcceptsVisual), defaultValue: _ => true);
    
    public event EventHandler<DragEventArgs> Drop
    {
        add => AddHandler(DropEvent, value);
        remove => RemoveHandler(DropEvent, value);
    }
    
    public event EventHandler<DragEventArgs> Hover
    {
        add => AddHandler(HoverEvent, value);
        remove => RemoveHandler(HoverEvent, value);
    }

    public Predicate<Visual> AcceptsVisual
    {
        get => GetValue(AcceptsVisualProperty);
        set => SetValue(AcceptsVisualProperty, value);
    }
}