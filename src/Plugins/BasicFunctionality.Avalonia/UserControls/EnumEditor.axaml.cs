using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Laminar.PluginFramework.UserInterface;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace BasicFunctionality.Avalonia.UserControls;

public partial class EnumEditor : UserControl
{
    public EnumEditor()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        CBox.ItemsSource = (DataContext as UserInterface<EnumDropdown, object>)?.Value.GetType().GetEnumValues();
    }
}
