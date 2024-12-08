using System.Runtime.InteropServices;
using Avalonia.Controls;
using Laminar.Avalonia.Views.CustomTitleBars;

namespace Laminar.Avalonia.Views;

public partial class TitleBarView : UserControl
{
    public TitleBarView()
    {
        InitializeComponent();
        Content = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? new MacosTitleBar() : new WindowsTitleBar();
    }
}