using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Reactive;
using Laminar.PluginFramework.UserInterface;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace BasicFunctionality.Avalonia.UserControls;

public partial class StringDisplay : UserControl
{
    private UserInterface<StringViewer, string>? _interface;

    private double _previousNameTextLayoutWidth;
    
    public StringDisplay()
    {
        InitializeComponent();
        
        NameBlock.GetObservable(BoundsProperty).Subscribe(new AnonymousObserver<Rect>(_ =>
        {
            if (_previousNameTextLayoutWidth != NameBlock.TextLayout.Width)
            {
                MainGrid.ColumnDefinitions[1].MinWidth = NameBlock.TextLayout.Width;
                _previousNameTextLayoutWidth = NameBlock.TextLayout.Width;
            }
        }));
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (_interface is not null)
        {
            _interface.PropertyChanged -= DisplayValue_PropertyChanged;   
        }
        
        _interface = DataContext as UserInterface<StringViewer, string>;
        if (_interface is not null)
        {
            _interface.PropertyChanged += DisplayValue_PropertyChanged;
        }
        
        DisplayValue_PropertyChanged(this, new PropertyChangedEventArgs(nameof(ValueInterface<string>.Value)));
    }

    private void DisplayValue_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IDisplayValue.Value) || _interface is null) return;
        
        if (_interface.Value.Length > _interface.Definition.MaxStringLength)
        {
            ValueViewer.Text = _interface.Value[.._interface.Definition.MaxStringLength] + "...";
        }
        else
        {
            ValueViewer.Text = _interface.Value;
        }
    }
}
