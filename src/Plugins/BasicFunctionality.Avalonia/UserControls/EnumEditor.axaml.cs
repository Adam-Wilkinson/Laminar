using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Laminar.PluginFramework.UserInterface;

namespace BasicFunctionality.Avalonia.UserControls;

public partial class EnumEditor : UserControl
{
    public EnumEditor()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        CBox.ItemsSource = (DataContext as IDisplayValue)?.Value!.GetType().GetEnumValues();
    }
}
