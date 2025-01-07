using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Avalonia;

namespace BasicFunctionality.Avalonia;

public class Compact
{
    public static readonly AttachedProperty<double?> CutoffProperty = AvaloniaProperty.RegisterAttached<Compact, Visual, double?>("Cutoff");
    
    public static double? GetCutoff(Visual visual) => visual.GetValue(CutoffProperty);
    public static void SetCutoff(Visual visual, double value) => visual.SetValue(CutoffProperty, value);

    public static readonly AttachedProperty<Visual?> CutoffTargetProperty = AvaloniaProperty.RegisterAttached<Compact, Visual, Visual?>("CutoffTarget");
    public static Visual? GetCutoffTarget(Visual visual) => visual.GetValue(CutoffTargetProperty);
    public static void SetCutoffTarget(Visual visual, Visual? value) => visual.SetValue(CutoffTargetProperty, value);
    
    public static readonly AttachedProperty<bool> IsCompactProperty = AvaloniaProperty.RegisterAttached<Compact, Visual, bool>("IsCompact");
    public static bool GetIsCompact(Visual visual) => visual.GetValue(IsCompactProperty);
    public static void SetIsCompact(Visual visual, bool value) => visual.SetValue(IsCompactProperty, value);
    
    static Compact()
    {
        CutoffProperty.Changed.AddClassHandler<Visual>(CutoffChanged);
        CutoffTargetProperty.Changed.AddClassHandler<Visual>(CutoffChanged);
    }

    private static void CutoffChanged(Visual visual, AvaloniaPropertyChangedEventArgs e)
    {
        if (GetCutoffTarget(visual) is not { } targetVisual) return;
        
        var cutoff = GetCutoff(visual);
        
        SetIsCompact(visual, targetVisual.Bounds.Width < cutoff);
        
        visual[!IsCompactProperty] =
            targetVisual.GetObservable(Visual.BoundsProperty, rect => rect.Width < cutoff).ToBinding();
    }
}