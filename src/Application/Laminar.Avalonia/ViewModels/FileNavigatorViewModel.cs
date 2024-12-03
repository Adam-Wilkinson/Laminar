using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Laminar.Avalonia.Commands;
using Laminar.Avalonia.Shapes;
using Laminar.Contracts.UserData;
using Laminar.Contracts.UserData.FileNavigation;
using Laminar.Implementation.UserData.FileNavigation;
using Laminar.Implementation.UserData.FileNavigation.UserActions;

namespace Laminar.Avalonia.ViewModels;
public class FileNavigatorViewModel : ViewModelBase
{
    private readonly IStorageProvider _storageProvider;
    
    public FileNavigatorViewModel(
        IStorageProvider storageProvider, 
        IPersistentDataManager dataManager, 
        ILaminarStorageItemFactory storageItemFactory,
        LaminarCommandFactory commandFactory)
    {
        _storageProvider = storageProvider;
        ToggleEnable = commandFactory
            .DefineTool<ILaminarStorageItem>("Toggle Enabled",
                item => new StringBuilder(4).Append("Click to ").Append(item.IsEnabled ? "disable" : "enable")
                    .Append(" this ").Append(ItemTypeName(item)).ToString(),
                LaminarCommandSwitch.Template(instance => new Binding
                    { Source = instance.Parameter, Path = nameof(ILaminarStorageItem.IsEffectivelyEnabled) }),
                new KeyGesture(Key.E, KeyModifiers.Alt)).AsCommand(new ToggleEnabledParameterAction());
        
        AddItem = commandFactory
            .DefineTool<ILaminarStorageFolder>("Add item", "Add item", 
                LaminarCommandIcon.Template(PathData.AddIcon), new KeyGesture(Key.A, KeyModifiers.Alt))
            .AsToolbox(
                commandFactory.DefineTool<ILaminarStorageFolder>("Add folder", "Add folder",
                        LaminarCommandIcon.Template(PathData.AddFolderIcon))
                    .AsCommand(new AddStorageItemParameterAction<ILaminarStorageItem>(storageItemFactory)),
                commandFactory.DefineTool<ILaminarStorageFolder>("Add script", "Add script",
                        LaminarCommandIcon.Template(PathData.AddScriptIcon))
                    .AsCommand(new AddStorageItemParameterAction<ILaminarStorageItem>(storageItemFactory)));
        
        DeleteItem = commandFactory
            .DefineTool<ILaminarStorageItem>("Delete Item", "Delete Item",
                LaminarCommandIcon.Template(PathData.DeleteIcon), new KeyGesture(Key.Delete))
            .AsCommand(new DeleteStorageItemParameterAction<ILaminarStorageItem>(storageItemFactory));
        
        RenameItem = commandFactory
            .DefineTool<ILaminarStorageItem>("Rename Item", item => $"Rename {ItemTypeName(item)}", 
                LaminarCommandIcon.Template(PathData.RenameIcon), new KeyGesture(Key.R, KeyModifiers.Control))
            .AsToolbox();
        
        RootFiles = [ new LaminarStorageFolder(Path.Combine(dataManager.Path, "Default"), storageItemFactory) ];
        FolderQuickAccess = [RenameItem, DeleteItem, AddItem, ToggleEnable];
    }

    public LaminarTool ToggleEnable { get; }

    public LaminarTool AddItem { get; }

    public LaminarTool DeleteItem { get; }

    public LaminarTool RenameItem { get; }

    [Serialize]
    public ObservableCollection<ILaminarStorageFolder> RootFiles { get; set; }

    public ObservableCollection<LaminarTool> FolderQuickAccess { get; }

    public void OpenFilePicker()
    {
        _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { AllowMultiple = false, Title = "Pick a file!"});
    }

    private static string ItemTypeName(ILaminarStorageItem item) => item switch
    {
        LaminarStorageFolder => "folder",
        _ => "item",
    };
}