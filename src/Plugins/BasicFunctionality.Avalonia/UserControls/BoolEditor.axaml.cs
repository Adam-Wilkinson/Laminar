using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Reactive;

namespace BasicFunctionality.Avalonia.UserControls;

public partial class BoolEditor : UserControl
{
    private double _previousTextLayoutWidth;
    
    public BoolEditor()
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
