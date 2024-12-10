using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Data;
using Avalonia.Input;
using Laminar.Avalonia.ToolSystem;
using Laminar.Avalonia.Shapes;
using Laminar.Contracts.UserData;
using Laminar.Contracts.UserData.FileNavigation;
using Laminar.Implementation.UserData.FileNavigation;
using Laminar.Implementation.UserData.FileNavigation.UserActions;

namespace Laminar.Avalonia.ViewModels;
public class FileNavigatorViewModel : ViewModelBase
{
    public FileNavigatorViewModel(IPersistentDataManager dataManager, ILaminarStorageItemFactory storageItemFactory, LaminarToolFactory toolFactory)
    {
        ToggleEnable = toolFactory
            .DefineTool<ILaminarStorageItem>("Toggle Enabled", LaminarToolSwitchIcon.CreateTemplate(
                instance => new Binding { Source = instance.Parameter, Path = nameof(ILaminarStorageItem.IsEffectivelyEnabled) }), 
                item => $"{(item.IsEnabled ? "Disable" : "Enable")} this {ItemTypeName(item)}", 
                new KeyGesture(Key.E, KeyModifiers.Alt))
            .AsCommand(new ToggleEnabledParameterAction());
        
        AddItem = toolFactory
            .DefineTool<ILaminarStorageFolder>("Add item", LaminarToolGeometryIcon.CreateTemplate(PathData.AddIcon))
            .AsToolbox(
                toolFactory
                    .DefineTool<ILaminarStorageFolder>("Add folder", LaminarToolGeometryIcon.CreateTemplate(PathData.FolderIcon), gesture: new KeyGesture(Key.S, KeyModifiers.Alt))
                    .AsCommand(new AddStorageItemParameterAction<ILaminarStorageFolder>(storageItemFactory)),
                toolFactory
                    .DefineTool<ILaminarStorageFolder>("Add script", LaminarToolGeometryIcon.CreateTemplate(PathData.ScriptIcon))
                    .AsCommand(new AddStorageItemParameterAction<LaminarStorageFile>(storageItemFactory)));
        
        DeleteItem = toolFactory
            .DefineTool<ILaminarStorageItem>("Delete Item", LaminarToolGeometryIcon.CreateTemplate(PathData.DeleteIcon), item => $"Delete {ItemTypeName(item)}", new KeyGesture(Key.Delete))
            .AsCommand(new DeleteStorageItemParameterAction<ILaminarStorageItem>(storageItemFactory));
        
        RenameItem = toolFactory
            .DefineTool<ILaminarStorageItem>("Rename Item", LaminarToolGeometryIcon.CreateTemplate(PathData.RenameIcon), item => $"Rename {ItemTypeName(item)}", new KeyGesture(Key.R, KeyModifiers.Control))
            .AsCommand(item => item.NeedsName = true);
        
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
    }

    private static string ItemTypeName(ILaminarStorageItem item) => item switch
    {
        LaminarStorageFolder => "folder",
        _ => "item",
    };

    public class FileNavigatorItem(ILaminarStorageItem item)
    {
        public ILaminarStorageItem StorageItem { get; } = item;
        
        public required ObservableCollection<FileNavigatorItem> Children { get; init; }

        public required ObservableCollection<LaminarTool> QuickAccess { get; init; }
    }
}