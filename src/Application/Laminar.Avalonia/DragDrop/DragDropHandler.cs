using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Laminar.Avalonia.DragDrop;

public class DragDropHandler
{
    public static readonly AttachedProperty<MouseButton?> TriggerMouseButtonProperty = AvaloniaProperty.RegisterAttached<DragDropHandler, Control, MouseButton?>("TriggerMouseButton");
    public static MouseButton? GetTriggerMouseButton(AvaloniaObject control) => control.GetValue(TriggerMouseButtonProperty);
    public static void SetTriggerMouseButton(AvaloniaObject control, MouseButton? mouseButton) => control.SetValue(TriggerMouseButtonProperty, mouseButton);

    private static DragEventArgs? _hoverArgs;
    private static Point? _clickOffset;
    private static ITransform? _controlOriginalTransform;
    private static bool? _controlIsClipToBounds;
    private static int? _controlZIndex;
    
    static DragDropHandler()
    {
        TriggerMouseButtonProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<MouseButton?>>(TriggerMouseButtonChanged));
    }

    public static DragDropDebugRenderer? DebugRenderer { get; set; }

    private static void TriggerMouseButtonChanged(AvaloniaPropertyChangedEventArgs<MouseButton?> e)
    {
        if (e.Sender is not Interactive inputElementSender)
        {
            throw new Exception($"Property {nameof(TriggerMouseButtonProperty)} is only valid on objects of type {typeof(Interactive)}");
        }
        
        inputElementSender.AddHandler(InputElement.PointerPressedEvent, InputElementSender_PointerPressed);
        inputElementSender.AddHandler(InputElement.PointerReleasedEvent, InputElementSender_PointerReleased);
        inputElementSender.AddHandler(InputElement.PointerMovedEvent, InputElementSender_PointerMoved);
    }

    private static void InputElementSender_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_hoverArgs is null || !_clickOffset.HasValue || _controlOriginalTransform is null)
        {
            return;
        }
        
        e.Handled = true;
        
        var currentClickOffset = e.GetCurrentPoint(null).Position; 
        var transform = TransformOperations.CreateBuilder(2);
        transform.AppendMatrix(_controlOriginalTransform.Value);
        transform.AppendTranslate(currentClickOffset.X - _clickOffset.Value.X, currentClickOffset.Y - _clickOffset.Value.Y);
        _hoverArgs.DraggingVisual.RenderTransform = transform.Build();

        var oldHoverInteractive = _hoverArgs.HoverOverInteractive;
        var oldHoverReceptacleTag = _hoverArgs.ReceptacleTag;
        ExecuteDragEventAtPointer(e, _hoverArgs);
        if (oldHoverInteractive is not null && (oldHoverInteractive != _hoverArgs.HoverOverInteractive || oldHoverReceptacleTag != _hoverArgs.ReceptacleTag))
        {
            oldHoverInteractive.RaiseEvent(DragEventArgs.HoverLeave(_hoverArgs.DraggingVisual, _hoverArgs.OriginalClickEventArgs, currentHoverInteractive: oldHoverInteractive, receptacleTag: oldHoverReceptacleTag));
        }
    }

    private static void InputElementSender_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control senderControl || !PointerHasMouseButton(e.GetCurrentPoint(null), GetTriggerMouseButton(senderControl)))
        {
            return;
        }

        _clickOffset = e.GetCurrentPoint(null).Position;
        _controlIsClipToBounds = senderControl.ClipToBounds;
        senderControl.ClipToBounds = false;
        
        _controlZIndex = senderControl.ZIndex;
        senderControl.ZIndex = int.MaxValue;

        _controlOriginalTransform = senderControl.RenderTransform ?? new TranslateTransform();
        _hoverArgs = DragEventArgs.HoverEnter(senderControl, e);
        
        e.Handled = true;
    }

    private static void InputElementSender_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_hoverArgs is null || _controlOriginalTransform is null || _clickOffset is null)
        {
            return;
        }

        e.Handled = true;
        e.PreventGestureRecognition();
        DebugRenderer?.EndAll();
        
        ExecuteDragEventAtPointer(e, DragEventArgs.Drop(_hoverArgs.DraggingVisual, _hoverArgs.OriginalClickEventArgs));
        AnimateHome(_hoverArgs.DraggingVisual, _controlOriginalTransform);
        _clickOffset = null;
    }

    private static void ExecuteDragEventAtPointer(PointerEventArgs pointerEventArgs, DragEventArgs dragEvent)
    {
        dragEvent.Handled = false;
        var currentHoverVisual = dragEvent.HoverOverInteractive;
        var currentReceptacleTag = dragEvent.ReceptacleTag;
        
        if (TopLevel.GetTopLevel(dragEvent.DraggingVisual) is not { } topLevel) return;

        foreach (var visualAtPoint in topLevel.GetVisualsAt(pointerEventArgs.GetPosition(topLevel),
                     visual => visual != dragEvent.DraggingVisual))
        {
            if (DebugRenderer is not null && visualAtPoint is Control control)
            {
                DebugRenderer.EnsureAttached(control);
            }
            
            if (!DropHandler.GetDropAcceptor(visualAtPoint)
                    .AcceptDrop(visualAtPoint, pointerEventArgs, out var receptacleTag))
            {
                continue;
            }

            if (visualAtPoint == currentHoverVisual && receptacleTag == currentReceptacleTag)
            {
                dragEvent.HoverOverInteractive = currentHoverVisual;
                dragEvent.ReceptacleTag = dragEvent.ReceptacleTag;
                return;
            }
            
            if (visualAtPoint is Interactive interactiveAtPoint)
            {
                dragEvent.HoverOverInteractive = interactiveAtPoint;
                dragEvent.ReceptacleTag = receptacleTag;
                interactiveAtPoint.RaiseEvent(dragEvent);
            }

            if (dragEvent.Handled) return;
        }

        dragEvent.HoverOverInteractive = null;
        dragEvent.ReceptacleTag = null;
    }
    
    private static void AnimateHome(Visual visual, ITransform originalTransform)
    {
        var transformTransition = new TransformOperationsTransition()
        {
            Property = Visual.RenderTransformProperty, 
            Duration = new TimeSpan(0, 0, 0, 0, 300),
            Easing = new QuadraticEaseInOut()
        };
        
        visual.Transitions ??= [];
        visual.Transitions.Add(transformTransition);
        
        var transform = TransformOperations.CreateBuilder(2);
        transform.AppendMatrix(originalTransform.Value);
        transform.AppendTranslate(0, 0);
        visual.RenderTransform = transform.Build();

        _ = Task.Run(() =>
        {
            Thread.Sleep(transformTransition.Duration);
            Dispatcher.UIThread.Post(() =>
            {
                visual.Transitions?.Remove(transformTransition);
                visual.ClipToBounds = _controlIsClipToBounds!.Value;
                visual.ZIndex = _controlZIndex!.Value;
                visual.Transitions?.Remove(transformTransition); 
            });
        });
    }

    private static bool PointerHasMouseButton(PointerPoint pointer, MouseButton? button) => button switch
    {
        MouseButton.Left => pointer.Properties.IsLeftButtonPressed,
        MouseButton.Right => pointer.Properties.IsRightButtonPressed,
        MouseButton.Middle => pointer.Properties.IsMiddleButtonPressed,
        MouseButton.XButton1 => pointer.Properties.IsXButton1Pressed,
        MouseButton.XButton2 => pointer.Properties.IsXButton2Pressed,
        _ => false,
    };
}