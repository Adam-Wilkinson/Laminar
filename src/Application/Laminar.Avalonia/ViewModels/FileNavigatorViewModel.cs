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
        LaminarToolFactory toolFactory)
    {
        _storageProvider = storageProvider;
        ToggleEnable = toolFactory
            .DefineTool<ILaminarStorageItem>("Toggle Enabled",
                LaminarCommandSwitch.Template(instance => new Binding
                    { Source = instance.Parameter, Path = nameof(ILaminarStorageItem.IsEffectivelyEnabled) }),
                item => new StringBuilder(3).Append(item.IsEnabled ? "Disable" : "Enable").Append(" this ").Append(ItemTypeName(item)).ToString(),
                new KeyGesture(Key.E, KeyModifiers.Alt))
            .AsCommand(new ToggleEnabledParameterAction());
        
        AddItem = toolFactory
            .DefineTool<ILaminarStorageFolder>("Add item", LaminarCommandIcon.Template(PathData.AddIcon))
            .AsToolbox(
                toolFactory
                    .DefineTool<ILaminarStorageFolder>("Add folder", LaminarCommandIcon.Template(PathData.AddFolderIcon), gesture: new KeyGesture(Key.S, KeyModifiers.Alt))
                    .AsCommand(new AddStorageItemParameterAction<ILaminarStorageFolder>(storageItemFactory)),
                toolFactory
                    .DefineTool<ILaminarStorageFolder>("Add script", LaminarCommandIcon.Template(PathData.AddScriptIcon))
                    .AsCommand(new AddStorageItemParameterAction<LaminarStorageFile>(storageItemFactory)));
        
        DeleteItem = toolFactory
            .DefineTool<ILaminarStorageItem>("Delete Item", LaminarCommandIcon.Template(PathData.DeleteIcon),
                item => $"Delete {ItemTypeName(item)}", new KeyGesture(Key.Delete))
            .AsCommand(new DeleteStorageItemParameterAction<ILaminarStorageItem>(storageItemFactory));
        
        RenameItem = toolFactory
            .DefineTool<ILaminarStorageItem>("Rename Item", LaminarCommandIcon.Template(PathData.RenameIcon), 
                item => $"Rename {ItemTypeName(item)}", new KeyGesture(Key.R, KeyModifiers.Control))
            .AsToolbox();
        
        RootFiles = [ new LaminarStorageFolder(Path.Combine(dataManager.Path, "Default"), storageItemFactory) ];
        FolderQuickAccess = [RenameItem, DeleteItem, AddItem, ToggleEnable];
        FileQuickAssess = [RenameItem, DeleteItem, ToggleEnable];
    }

    public LaminarTool ToggleEnable { get; }

    public LaminarTool AddItem { get; }

    public LaminarTool DeleteItem { get; }

    public LaminarTool RenameItem { get; }

    [Serialize]
    public ObservableCollection<ILaminarStorageFolder> RootFiles { get; set; }

    public ObservableCollection<LaminarTool> FolderQuickAccess { get; }
    
    public ObservableCollection<LaminarTool> FileQuickAssess { get; }

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