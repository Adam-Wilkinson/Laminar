using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Laminar.Avalonia.Commands;
using Laminar.Avalonia.Shapes;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement.FileNavigation;
using Laminar.Domain.Extensions;
using Laminar.Domain.Notification;

namespace Laminar.Avalonia.ViewModels;
public class FileNavigatorViewModel : ViewModelBase
{
    private readonly IStorageProvider _storageProvider;
    
    public FileNavigatorViewModel(IStorageProvider storageProvider, 
        IPersistentDataManager dataManager, 
        LaminarCommandFactory commandFactory)
    {
        _storageProvider = storageProvider;
        ToggleEnable = commandFactory.CreateParameterCommand(
            "Toggle Enabled",
            item => item.IsEnabled = !item.IsEnabled,
            item => item.IsEnabled = !item.IsEnabled,
            ToggleEnabledDataTemplate,
            new ReactiveFunc<ILaminarStorageItem, string>(item => new StringBuilder(4)
                .Append("Click to ")
                .Append(item.IsEffectivelyEnabled ? "disable" : "enable")
                .Append(" this ")
                .Append(ItemTypeName(item)).ToString()),
            new ReactiveFunc<ILaminarStorageItem, bool>(item => item.ParentIsEffectivelyEnabled),
            new KeyGesture(Key.E, KeyModifiers.Alt)
        );
        AddItem = commandFactory.CreateParameterCommand(
            "Add Item",
            item => false,
            null,
            LaminarCommandIcon.Template(PathData.AddIcon),
            new ReactiveFunc<LaminarStorageFolder, string>(_ => "Add a new file or folder"));
        DeleteItem = commandFactory.CreateParameterCommand(
            "Delete Item",
            item => false,
            null,
            LaminarCommandIcon.Template(PathData.DeleteIcon),
            new ReactiveFunc<ILaminarStorageItem, string>(item => $"Delete {ItemTypeName(item)}"));
        RenameItem = commandFactory.CreateParameterCommand(
            "Rename Item",
            item => false,
            null,
            LaminarCommandIcon.Template(PathData.RenameIcon),
            new ReactiveFunc<ILaminarStorageItem,string>(item => $"Rename {ItemTypeName(item)}"));
        RootFiles = [ new LaminarStorageFolder(Path.Combine(dataManager.Path, "Default")) ];
        FolderQuickAccess = [RenameItem, DeleteItem, AddItem, ToggleEnable];
    }

    public LaminarCommand ToggleEnable { get; }

    public LaminarCommand AddItem { get; }

    public LaminarCommand DeleteItem { get; }

    public LaminarCommand RenameItem { get; }

    [Serialize]
    public ObservableCollection<ILaminarStorageItem> RootFiles { get; set; }

    public ObservableCollection<LaminarCommand> FolderQuickAccess { get; }

    public void OpenFilePicker()
    {
        _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { AllowMultiple = false, Title = "Pick a file!"});
    }

    private static readonly IDataTemplate ToggleEnabledDataTemplate =
        new FuncDataTemplate<LaminarCommandInstance>(
            _ => true,
            commandInstance =>
            {
                var newSwitch = new LaminarCommandSwitch
                {
                    [!LaminarCommandSwitch.IsOnProperty] =
                        new Binding
                        {
                            Source = commandInstance.Parameter, Path = nameof(ILaminarStorageItem.IsEffectivelyEnabled)
                        },
                };

                newSwitch.Tapped += (_, _) => commandInstance.Execute();

                return newSwitch;
            });

    private static string ItemTypeName(ILaminarStorageItem item) => item switch
    {
        LaminarStorageFolder folder => "folder",
        _ => "item",
    };
}