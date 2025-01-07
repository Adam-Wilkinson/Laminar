using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Reactive;

namespace BasicFunctionality.Avalonia.UserControls;

public partial class NumberEditor : UserControl
{
    private double _previousTextLayoutWidth;
    
    public NumberEditor()
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
    }
}
