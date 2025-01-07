using Avalonia.Controls;

namespace BasicFunctionality.Avalonia.UIViewer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public void Invalid()
    {
        this.StringEditor.InvalidChain();
    }
}