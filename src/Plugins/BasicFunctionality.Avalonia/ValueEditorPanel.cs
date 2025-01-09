using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace BasicFunctionality.Avalonia;

public class ValueEditorPanel : Panel
{
    public static readonly AttachedProperty<ValueEditorChildPosition> PositionProperty = AvaloniaProperty.RegisterAttached<ValueEditorPanel, Control, ValueEditorChildPosition>("Position");
    public static ValueEditorChildPosition GetPosition(Control control) => control.GetValue(PositionProperty);
    public static void SetPosition(Control control, ValueEditorChildPosition position) => control.SetValue(PositionProperty, position);

    private static readonly AttachedProperty<double> LeftControlOffsetProperty = AvaloniaProperty.RegisterAttached<ValueEditorPanel, Control, double>("LeftControlOffset");
    private static double GetLeftControlOffset(Control control) => control.GetValue(LeftControlOffsetProperty);
    private static void SetLeftControlOffset(Control control, double offset) => control.SetValue(LeftControlOffsetProperty, offset);
    
    public static readonly StyledProperty<double> IsCompactCutoffProperty = AvaloniaProperty.Register<ValueEditorPanel, double>(nameof(IsCompactCutoff), defaultValue: double.NaN);
    public double IsCompactCutoff
    {
        get => GetValue(IsCompactCutoffProperty);
        set => SetValue(IsCompactCutoffProperty, value);
    }

    public static readonly DirectProperty<ValueEditorPanel, bool> IsCompactProperty = AvaloniaProperty.RegisterDirect<ValueEditorPanel, bool>(nameof(IsCompact), o => o.IsCompact);
    public bool IsCompact
    {
        get => _isCompact;
        set => SetAndRaise(IsCompactProperty, ref _isCompact, value); 
    }

    public static readonly StyledProperty<double> LeftNameplateMarginProperty = AvaloniaProperty.Register<ValueEditorPanel, double>(nameof(LeftNameplateMargin), defaultValue: 8);
    public double LeftNameplateMargin
    {
        get => GetValue(LeftNameplateMarginProperty);
        set => SetValue(LeftNameplateMarginProperty, value);
    }
    
    private readonly Dictionary<Control, (int startingDesignator, int endingIndicator)> _previousPositionIndicators = [];
    
    
    private double _nameplateMeasuredDesiredWidth;
    private double _compactValueMeasuredDesiredWidth;
    private double _expandedValueMeasuredDesiredWidth;
    private bool _isCompact;

    private readonly DoubleTransition _leftControlOffsetTransition = new() { Property = LeftControlOffsetProperty, Duration = TimeSpan.FromMilliseconds(150), Easing = new QuadraticEaseInOut() };

    static ValueEditorPanel()
    {
        AffectsParentMeasure<ValueEditorPanel>(PositionProperty, LeftControlOffsetProperty);
        AffectsMeasure<ValueEditorPanel>(IsCompactProperty, IsCompactCutoffProperty, LeftNameplateMarginProperty);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var isCompact = finalSize.Width < (double.IsNaN(IsCompactCutoff) ? LeftNameplateMargin + _nameplateMeasuredDesiredWidth + _expandedValueMeasuredDesiredWidth : IsCompactCutoff);
        Span<double> positionIndicatorValues = stackalloc double[4];
        positionIndicatorValues[0] = 0;
        positionIndicatorValues[1] = LeftNameplateMargin;

        positionIndicatorValues[2] = finalSize.Width > _nameplateMeasuredDesiredWidth * 2 + LeftNameplateMargin
            ? (finalSize.Width + LeftNameplateMargin) / 2
            : LeftNameplateMargin + _nameplateMeasuredDesiredWidth;

        positionIndicatorValues[3] = finalSize.Width;
        
        foreach (var control in Children)
        {
            var (newStartingDesignator, newEndingDesignator) = GetPositionDesignators(control, isCompact);
            if (_previousPositionIndicators.TryGetValue(control, out var designators))
            {
                var (oldStartingDesignator, _) = designators;
                if (oldStartingDesignator != newStartingDesignator)
                {
                    ChangeOffsetAnimation(control, positionIndicatorValues[oldStartingDesignator] - positionIndicatorValues[newStartingDesignator]);
                }
            }
            _previousPositionIndicators[control] = (newStartingDesignator, newEndingDesignator);
            var leftControlOffset = GetLeftControlOffset(control);
            control.Arrange(new Rect(positionIndicatorValues[newStartingDesignator] + leftControlOffset, 0, Math.Max(control.DesiredSize.Width, positionIndicatorValues[newEndingDesignator] - positionIndicatorValues[newStartingDesignator] - leftControlOffset), finalSize.Height));
        }

        IsCompact = isCompact;
        
        return finalSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var maxHeight = 0.0;

        _nameplateMeasuredDesiredWidth = 0;
        _compactValueMeasuredDesiredWidth = 0;

        if (!IsCompact)
        {
            _expandedValueMeasuredDesiredWidth = 0;
        }

        foreach (var control in Children)
        {
            control.Measure(availableSize.WithWidth(double.PositiveInfinity));
            maxHeight = Math.Max(maxHeight, control.DesiredSize.Height);
            if (GetPosition(control) == ValueEditorChildPosition.Nameplate)
            {
                _nameplateMeasuredDesiredWidth = Math.Max(_nameplateMeasuredDesiredWidth, control.DesiredSize.Width);
            }
        
            if (GetPosition(control) == ValueEditorChildPosition.Value && !IsCompact)
            {
                _expandedValueMeasuredDesiredWidth = Math.Max(_expandedValueMeasuredDesiredWidth, control.DesiredSize.Width);
            }
        }

        var desiredWidth = LeftNameplateMargin + _nameplateMeasuredDesiredWidth + _compactValueMeasuredDesiredWidth;
        return new Size(desiredWidth, maxHeight);
    }

    private void ChangeOffsetAnimation(Control control, double offsetChange)
    {
        control.Transitions ??= [];
        var currentLeftControlOffset = GetLeftControlOffset(control);
        control.Transitions.Remove(_leftControlOffsetTransition);
        SetLeftControlOffset(control, currentLeftControlOffset + offsetChange);
        control.Transitions.Add(_leftControlOffsetTransition);
        SetLeftControlOffset(control, 0);
    }

    private static (int, int) GetPositionDesignators(Control control, bool isCompact) => GetPosition(control) switch
    {
        ValueEditorChildPosition.Nameplate => (1, 2),
        ValueEditorChildPosition.Value => (2, 3),
        ValueEditorChildPosition.SpansWhenCompact => (isCompact ? 0 : 2, 3),
        _ => (0, 3)
    };
}

public enum ValueEditorChildPosition
{
    Nameplate = 0,
    SpansWhenCompact = 1,
    Value = 2,
}