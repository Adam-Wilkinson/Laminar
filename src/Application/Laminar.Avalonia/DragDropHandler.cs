using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
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

namespace Laminar.Avalonia;

public class DragDropHandler
{
    public static readonly AttachedProperty<object> DragSourceControlProperty = AvaloniaProperty.RegisterAttached<DragDropHandler, Control, object>("DragSourceControl");
    public static object GetDragSourceControl(AvaloniaObject control) => control.GetValue(DragSourceControlProperty);
    public static void SetDragSourceControl(AvaloniaObject control, object obj) => control.SetValue(DragSourceControlProperty, obj);

    public static readonly AttachedProperty<ICommand> DragEndCommandProperty = AvaloniaProperty.RegisterAttached<DragDropHandler, Interactive, ICommand>("DragEndCommand");
    public static ICommand GetDragEndCommand(Interactive interactive) => interactive.GetValue(DragEndCommandProperty);
    public static void SetDragEndCommand(Interactive interactive, ICommand command) => interactive.SetValue(DragEndCommandProperty, command);

    public static readonly AttachedProperty<ICommand> DragHoverCommandProperty = AvaloniaProperty.RegisterAttached<DragDropHandler, Interactive, ICommand>("DragHoverCommand");
    public static ICommand GetDragHoverCommand(Interactive interactive) => interactive.GetValue(DragHoverCommandProperty);
    public static void SetDragHoverCommand(Interactive interactive, ICommand command) => interactive.SetValue(DragHoverCommandProperty, command);

    private static object? _currentDragObject;
    private static Point? _clickOffset;
    private static ITransform? _controlOriginalTransform;
    private static bool? _controlIsClipToBounds;
    private static int? _controlZIndex;
    
    static DragDropHandler()
    {
        DragSourceControlProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<object>>(DragSourcePropertyChanged));
    }

    private static void DragSourcePropertyChanged(AvaloniaPropertyChangedEventArgs<object> e)
    {
        if (e.Sender is not Interactive inputElementSender)
        {
            throw new Exception($"Property {nameof(DragSourceControlProperty)} is only valid on objects of type {typeof(Interactive)}");
        }

        inputElementSender.AddHandler(InputElement.PointerPressedEvent, InputElementSender_PointerPressed);
        inputElementSender.AddHandler(InputElement.PointerReleasedEvent, InputElementSender_PointerReleased);
        inputElementSender.AddHandler(InputElement.PointerMovedEvent, InputElementSender_PointerMoved);
    }

    private static void InputElementSender_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_currentDragObject is null || sender is not Visual senderVisual || !_clickOffset.HasValue || _controlOriginalTransform is null)
        {
            return;
        }
        
        var currentClickOffset = e.GetPosition(senderVisual.GetVisualParent()); 
        var transform = TransformOperations.CreateBuilder(2);
        transform.AppendMatrix(_controlOriginalTransform.Value);
        transform.AppendTranslate(currentClickOffset.X - _clickOffset.Value.X, currentClickOffset.Y - _clickOffset.Value.Y);
        senderVisual.RenderTransform = transform.Build();

        e.Handled = true;
    }

    private static void InputElementSender_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control senderControl || !e.GetCurrentPoint(senderControl).Properties.IsLeftButtonPressed || senderControl.GetValue(DragSourceControlProperty) is not { } dragControl)
        {
            return;
        }

        _clickOffset = e.GetPosition(senderControl.GetVisualParent());
        _currentDragObject = dragControl;
        _controlIsClipToBounds = senderControl.ClipToBounds;
        senderControl.ClipToBounds = false;
        
        _controlZIndex = senderControl.ZIndex;
        senderControl.ZIndex = int.MaxValue;

        _controlOriginalTransform = senderControl.RenderTransform ?? new TranslateTransform();
        
        e.Handled = true;
    }

    private static async void InputElementSender_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_currentDragObject is null || sender is not Visual senderVisual || _controlOriginalTransform is null || _clickOffset is null)
        {
            return;
        }

        e.Handled = true;
        e.PreventGestureRecognition();
        
        var transformTransition = new TransformOperationsTransition()
        {
            Property = Visual.RenderTransformProperty, 
            Duration = new TimeSpan(0, 0, 0, 0, 300),
            Easing = new QuadraticEaseInOut()
        };
        
        senderVisual.Transitions ??= [];
        senderVisual.Transitions.Add(transformTransition);
        
        var transform = TransformOperations.CreateBuilder(2);
        transform.AppendMatrix(_controlOriginalTransform.Value);
        transform.AppendTranslate(0, 0);
        senderVisual.RenderTransform = transform.Build();

        _ = Task.Run(() =>
        {
            Thread.Sleep(transformTransition.Duration);
            Dispatcher.UIThread.Post(() =>
            {
                senderVisual.Transitions?.Remove(transformTransition);
                senderVisual.ClipToBounds = _controlIsClipToBounds!.Value;
                senderVisual.ZIndex = _controlZIndex!.Value;
                senderVisual.Transitions?.Remove(transformTransition); 
            });
        });
        _clickOffset = null;
    }

    private static void RemoveTransitions(Visual visualToRemove, TransitionBase transition, TimeSpan delay)
    {
    }
}