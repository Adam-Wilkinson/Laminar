using Avalonia.Controls;
using Laminar.Avalonia.Commands;
using Laminar.Implementation.UserData.FileNavigation;

namespace Laminar.Avalonia.ViewModels.Design;

public static class DesignViewModel
{
    private static readonly LaminarToolFactory MockToolFactory = new(new MockUserActionManager(), TopLevel.GetTopLevel(null));
    
    public static readonly FileNavigatorViewModel FileNavigator = new(new MockDataManager(), new LaminarStorageItemFactory(), MockToolFactory);
    
    public static readonly MainControlViewModel MainControl = new(FileNavigator);
    
    public static readonly TitleBarViewModel TitleBar = new(MockToolFactory);
    
    public static readonly MainWindowViewModel MainWindow = new(MainControl, TitleBar);

}