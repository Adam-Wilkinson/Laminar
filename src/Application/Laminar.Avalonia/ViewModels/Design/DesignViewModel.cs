namespace Laminar.Avalonia.ViewModels.Design;

public static class DesignViewModel
{
    public static readonly FileNavigatorViewModel FileNavigator = new FileNavigatorViewModel(null, null);
    
    public static readonly MainControlViewModel MainControl = new MainControlViewModel(FileNavigator);
    
    public static readonly MainWindowViewModel MainWindow = new MainWindowViewModel(MainControl);
}