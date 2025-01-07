using Avalonia;
using Avalonia.Controls;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;

namespace BasicFunctionality.Avalonia;

public class CompositionAnimator
{
    private static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(200);
    
    public static readonly AttachedProperty<TransitionSetting> EnableOffsetAnimationProperty = AvaloniaProperty.RegisterAttached<CompositionAnimator, Control, TransitionSetting>("EnableOffsetAnimation");
    public static TransitionSetting GetEnableOffsetAnimation(Control element) => element.GetValue(EnableOffsetAnimationProperty);
    public static void SetEnableOffsetAnimation(Control element, TransitionSetting value) => element.SetValue(EnableOffsetAnimationProperty, value);
    
    static CompositionAnimator()
    {
        EnableOffsetAnimationProperty.Changed.AddClassHandler<Control>(OnEnableScaleShowAnimationChanged);
    }

    private static void OnEnableScaleShowAnimationChanged(Control control, AvaloniaPropertyChangedEventArgs eventArgs)
    {
        if (!control.IsLoaded)
        {
            control.Loaded += (_, _) =>
            {
                UpdateOffsetAnimation(control, GetEnableOffsetAnimation(control));
            };
        }
        else
        {
            UpdateOffsetAnimation(control, GetEnableOffsetAnimation(control));   
        }
    }

    private static void UpdateOffsetAnimation(Control control, TransitionSetting doOffsetAnimation)
    {
        var compositionVisual = ElementComposition.GetElementVisual(control);
        ArgumentNullException.ThrowIfNull(compositionVisual);

        var offsetTransitionAfter = doOffsetAnimation == TransitionSetting.OneTime ? TransitionSetting.None : doOffsetAnimation;
        var compositor = compositionVisual.Compositor;        
        var implicitAnimationCollection = compositor.CreateImplicitAnimationCollection();
        
        if (doOffsetAnimation != TransitionSetting.None)
        {
            // var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            // offsetAnimation.Target = "Offset";
            // offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            // offsetAnimation.Duration = AnimationDuration;
            // implicitAnimationCollection["Offset"] = offsetAnimation;
            
            var sizeAnimation = compositor.CreateVector3KeyFrameAnimation();
            sizeAnimation.Target = "Offset";
            sizeAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            sizeAnimation.Duration = AnimationDuration;
            implicitAnimationCollection["Offset"] = sizeAnimation;
        }
        
        compositionVisual.ImplicitAnimations = implicitAnimationCollection;

        if (offsetTransitionAfter != doOffsetAnimation)
        {
            var dispatcherTimer = new DispatcherTimer(AnimationDuration, DispatcherPriority.Background, (_, _) =>
            {
                SetEnableOffsetAnimation(control, offsetTransitionAfter);
            });
            dispatcherTimer.Start();
        }
    }
}

public enum TransitionSetting
{
    None,
    All,
    OneTime,
}