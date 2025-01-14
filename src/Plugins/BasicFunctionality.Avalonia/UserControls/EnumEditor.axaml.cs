using System.Diagnostics;
using Avalonia.Controls;
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
        CBox.ItemsSource = (DataContext as InterfaceData<EnumDropdown, object>)?.Value.GetType().GetEnumValues();
    }
}
