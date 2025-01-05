using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Laminar.PluginFramework.UserInterface;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace BasicFunctionality.Avalonia.UserControls;

public partial class StringDisplay : UserControl
{
    private IDisplayValue? _displayValue;
    private StringViewer? _interfaceDefinition;

    public StringDisplay()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        DataContextChanged += StringDisplay_DataContextChanged;
    }

    private void StringDisplay_DataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is not IDisplayValue displayValue) return;
        _displayValue = displayValue;
        _interfaceDefinition = displayValue.InterfaceDefinition as StringViewer;
        displayValue.PropertyChanged += DisplayValue_PropertyChanged;
        DisplayValue_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(IDisplayValue.Value)));
    }

    private void DisplayValue_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IDisplayValue.Value)) return;
        
        if (_displayValue!.Value!.ToString()!.Length > _interfaceDefinition!.MaxStringLength)
        {
            PART_MainDisplay.Text = _displayValue.Value.ToString()![.._interfaceDefinition.MaxStringLength] + "...";
        }
        else
        {
            PART_MainDisplay.Text = _displayValue.Value.ToString();
        }
    }
}
