using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Laminar.Avalonia.DragDrop;

public class DragEventArgs : RoutedEventArgs
{
    public static DragEventArgs Hover(Visual draggingControl, PointerPressedEventArgs clickEvent, Visual? currentHoverControl = null, object? recepticleTag = null)
        => new DragEventArgs(DropHandler.HoverEvent, draggingControl)
            { ClickEvent = clickEvent, DraggingVisual = draggingControl, HoverOverVisual = currentHoverControl, RecepticleTag = recepticleTag };

    public static DragEventArgs Drop(Visual draggingControl, PointerPressedEventArgs clickEvent, Visual? currentHoverControl = null, object? recepticleTag = null)
        => new DragEventArgs(DropHandler.DropEvent, draggingControl)
            { ClickEvent = clickEvent, DraggingVisual = draggingControl, HoverOverVisual = currentHoverControl, RecepticleTag = recepticleTag };
    
    private DragEventArgs(RoutedEvent<DragEventArgs> routedEvent, Visual draggingControl)
        :base(routedEvent, draggingControl)
    {
    }

    public required Visual DraggingVisual { get; init; }

    public required PointerPressedEventArgs ClickEvent { get; init; }

    public Visual? HoverOverVisual { get; set; }

    public object? RecepticleTag { get; set; }
}