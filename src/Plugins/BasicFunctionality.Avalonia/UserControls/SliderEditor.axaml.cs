using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace BasicFunctionality.Avalonia.UserControls;

public partial class SliderEditor : UserControl
{
    private double _previousTextLayoutWidth;

    private Point? _pointerDownPoint;
    private double _sliderPositionBeforePointerDown;
    
    static SliderEditor()
    {
        PointerPressedEvent.AddClassHandler<Slider>((slider, args) =>
        {
            if (slider.FindAncestorOfType<SliderEditor>() is { } sliderEditor)
            {
                sliderEditor._pointerDownPoint = args.GetPosition(sliderEditor);
            }
        }, handledEventsToo: true);

        PointerReleasedEvent.AddClassHandler<Slider>((slider, args) =>
        {
            if (slider.FindAncestorOfType<SliderEditor>() is { } sliderEditor)
            {
                if (sliderEditor._pointerDownPoint == args.GetPosition(sliderEditor))
                {
                    slider.Value = sliderEditor._sliderPositionBeforePointerDown;
                    sliderEditor.NumberEntry.Value = (decimal)sliderEditor.MainSlider.Value;
                    sliderEditor.SetIsEnteringValue(true);
                }

                sliderEditor._sliderPositionBeforePointerDown = slider.Value;
            }
        }, handledEventsToo: true);
    }
    
    public SliderEditor()
    {
        InitializeComponent();
        
        NameBlock.GetObservable(BoundsProperty).Subscribe(new AnonymousObserver<Rect>(_ =>
        {
            if (_previousTextLayoutWidth != NameBlock.TextLayout.Width)
            {
                MainGrid.ColumnDefinitions[1].MinWidth = NameBlock.TextLayout.Width;
                _previousTextLayoutWidth = NameBlock.TextLayout.Width;
            }
        }));
        
        MainSlider.GetObservable(Grid.ColumnProperty).Subscribe(new AnonymousObserver<int>(_ =>
        {
            CompositionAnimator.SetEnableOffsetAnimation(MainSlider, TransitionSetting.OneTime);
        }));

        MainSlider.PointerWheelChanged += (s, e) =>
        {
            MainSlider.Value += e.Delta.Y - e.Delta.X;
        };

        NumberEntry.Classes.CollectionChanged += (s, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Contains(":focus-within"))
            {
                if (NumberEntry.Value.HasValue)
                {
                    MainSlider.Value = (double)NumberEntry.Value;
                }
                SetIsEnteringValue(false);
            }
        };
        
        NumberEntry.KeyUp += (_, args) =>
        {
            Debug.WriteLine(args.Key);
            if (args.Key == Key.Enter)
            {
                if (NumberEntry.Value.HasValue)
                {
                    MainSlider.Value = (double)NumberEntry.Value;
                }
                SetIsEnteringValue(false);
            }

            if (args.Key == Key.Escape)
            {
                SetIsEnteringValue(false);
            }
        };
    }
    
    private void SetIsEnteringValue(bool isEnteringValue)
    {
        MainSlider.IsVisible = !isEnteringValue;
        NumberEntry.IsVisible = isEnteringValue;
        NumberDisplay.IsVisible = !isEnteringValue;

        if (isEnteringValue)
        {
            NumberEntry.Value = (decimal)MainSlider.Value;
            NumberEntry.Focus();
        }
    }
}
