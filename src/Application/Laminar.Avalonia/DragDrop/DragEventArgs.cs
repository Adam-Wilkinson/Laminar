using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Laminar.Avalonia.DragDrop;

public class DragEventArgs : RoutedEventArgs
{
    public static DragEventArgs HoverEnter(Visual draggingVisual, PointerPressedEventArgs clickEvent, Interactive? currentHoverInteractive = null, object? receptacleTag = null)
        => new DragEventArgs(DropHandler.HoverEnterEvent, draggingVisual)
            { OriginalClickEventArgs = clickEvent, DraggingVisual = draggingVisual, HoverOverInteractive = currentHoverInteractive, ReceptacleTag = receptacleTag };

    public static DragEventArgs HoverLeave(Visual draggingVisual, PointerPressedEventArgs clickEvent, Interactive? currentHoverInteractive = null, object? receptacleTag = null) 
        => new DragEventArgs(DropHandler.HoverLeaveEvent, draggingVisual)
            { OriginalClickEventArgs = clickEvent, DraggingVisual = draggingVisual, HoverOverInteractive = currentHoverInteractive, ReceptacleTag = receptacleTag };
    
    public static DragEventArgs Drop(Visual draggingVisual, PointerPressedEventArgs clickEvent, Interactive? currentHoverInteractive = null, object? receptacleTag = null)
        => new DragEventArgs(DropHandler.DropEvent, draggingVisual)
            { OriginalClickEventArgs = clickEvent, DraggingVisual = draggingVisual, HoverOverInteractive = currentHoverInteractive, ReceptacleTag = receptacleTag };
    
    private DragEventArgs(RoutedEvent<DragEventArgs> routedEvent, Visual draggingControl)
        :base(routedEvent, draggingControl)
    {
    }

    public required Visual DraggingVisual { get; init; }

    public required PointerPressedEventArgs OriginalClickEventArgs { get; init; }

    public Interactive? HoverOverInteractive { get; set; }

    public object? ReceptacleTag { get; set; }
}