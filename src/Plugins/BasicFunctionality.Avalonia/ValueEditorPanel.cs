using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace BasicFunctionality.Avalonia;

public class ValueEditorPanel : Panel
{
    public static readonly AttachedProperty<ValueEditorChildPosition> PositionProperty = AvaloniaProperty.RegisterAttached<ValueEditorPanel, Control, ValueEditorChildPosition>("Position");
    public static ValueEditorChildPosition GetPosition(Control control) => control.GetValue(PositionProperty);
    public static void SetPosition(Control control, ValueEditorChildPosition position) => control.SetValue(PositionProperty, position);

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
    
    private readonly Dictionary<ValueEditorChildPosition, List<Control>> _sortedChildren = Enum.GetValues<ValueEditorChildPosition>().ToDictionary(x => x, x => new List<Control>());

    private double _nameplateMeasuredDesiredWidth;
    private double _compactValueMeasuredDesiredWidth;
    private double _expandedValueMeasuredDesiredWidth;
    private bool _isCompact;

    static ValueEditorPanel()
    {
        PositionProperty.Changed.AddClassHandler<Control>((control, args) =>
        {
            if (control.Parent is not ValueEditorPanel valueEditor) return;
            var (oldValue, newValue) = args.GetOldAndNewValue<ValueEditorChildPosition>();
            valueEditor._sortedChildren[oldValue].Remove(control);
            valueEditor._sortedChildren[newValue].Add(control);
            valueEditor.InvalidateMeasure();
        });
    }

    protected override void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        base.ChildrenChanged(sender, e);
        if (e.OldItems is not null)
        {
            foreach (Control control in e.OldItems)
            {
                _sortedChildren[GetPosition(control)].Remove(control);
            }
        }

        if (e.NewItems is not null)
        {
            foreach (Control control in e.NewItems)
            {
                _sortedChildren[GetPosition(control)].Add(control);
            }
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double actualNameplateWidth, actualValueWidth;
        
        if (finalSize.Width > _nameplateMeasuredDesiredWidth * 2 + LeftNameplateMargin)
        {
            actualNameplateWidth = (finalSize.Width - LeftNameplateMargin) / 2;
            actualValueWidth = (finalSize.Width - LeftNameplateMargin) / 2;
        }
        else
        {
            actualNameplateWidth = _nameplateMeasuredDesiredWidth;
            actualValueWidth = Math.Max(0, finalSize.Width - _nameplateMeasuredDesiredWidth - LeftNameplateMargin);
        }

        var actualNameplateEnd = actualNameplateWidth + LeftNameplateMargin;
        IsCompact = finalSize.Width < (double.IsNaN(IsCompactCutoff) ? LeftNameplateMargin + _nameplateMeasuredDesiredWidth + _expandedValueMeasuredDesiredWidth : IsCompactCutoff);
        
        var compactElementRect = IsCompact
            ? new Rect(0, 0, finalSize.Width, finalSize.Height)
            : new Rect(actualNameplateEnd, 0, actualValueWidth, finalSize.Height);
        
        foreach (var control in _sortedChildren[ValueEditorChildPosition.Nameplate])
        {
            control.Arrange(new Rect(LeftNameplateMargin, 0, actualNameplateWidth, finalSize.Height));
        }
        
        foreach (var control in _sortedChildren[ValueEditorChildPosition.Value])
        {
            control.Arrange(new Rect(actualNameplateEnd, 0, Math.Max(actualValueWidth, control.DesiredSize.Width), finalSize.Height));
        }
        
        foreach (var control in _sortedChildren[ValueEditorChildPosition.SpansWhenCompact])
        {
            control.Arrange(compactElementRect);
        }

        return finalSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var maxHeight = 0.0;

        _nameplateMeasuredDesiredWidth = 0;
        _compactValueMeasuredDesiredWidth = 0;
        
        foreach (var control in _sortedChildren[ValueEditorChildPosition.Nameplate])
        {
            control.Measure(availableSize);
            maxHeight = Math.Max(maxHeight, control.DesiredSize.Height);
            _nameplateMeasuredDesiredWidth = Math.Max(_nameplateMeasuredDesiredWidth, control.DesiredSize.Width);            
        }

        foreach (var control in _sortedChildren[ValueEditorChildPosition.Value])
        {
            control.Measure(availableSize.WithWidth(double.PositiveInfinity));
            maxHeight = Math.Max(maxHeight, control.DesiredSize.Height);
            if (IsCompact)
            {
                _compactValueMeasuredDesiredWidth = Math.Max(_compactValueMeasuredDesiredWidth, control.DesiredSize.Width);   
            }
            else
            {
                _expandedValueMeasuredDesiredWidth = Math.Max(_compactValueMeasuredDesiredWidth, control.DesiredSize.Width);
            }
        }

        foreach (var control in _sortedChildren[ValueEditorChildPosition.SpansWhenCompact])
        {
            control.Measure(availableSize);
            maxHeight = Math.Max(maxHeight, control.DesiredSize.Height);
            _compactValueMeasuredDesiredWidth = Math.Max(_compactValueMeasuredDesiredWidth, control.DesiredSize.Width);
        }

        var desiredWidth = LeftNameplateMargin + _nameplateMeasuredDesiredWidth + _compactValueMeasuredDesiredWidth;
        return new Size(desiredWidth, maxHeight);
    }
}

public enum ValueEditorChildPosition
{
    Nameplate = 0,
    SpansWhenCompact = 1,
    Value = 2,
}