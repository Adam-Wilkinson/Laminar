using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BasicFunctionality.Avalonia.UserControls;

public partial class ActionDisplay : UserControl
{
    public ActionDisplay()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
