using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BasicFunctionality.Avalonia.UserControls;

public partial class BoolEditor : UserControl
{
    public BoolEditor()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
