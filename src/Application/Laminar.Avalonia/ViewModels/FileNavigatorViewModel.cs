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
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement.FileNavigation;
using Laminar.Domain.Extensions;
using Laminar.Domain.Notification;

namespace Laminar.Avalonia.ViewModels;
public class FileNavigatorViewModel(
    IStorageProvider storageProvider, 
    IPersistentDataManager dataManager, 
    LaminarCommandFactory commandFactory) : ViewModelBase
{
    private readonly IStorageProvider _storageProvider = storageProvider;

    public LaminarCommand ToggleEnable { get; } = commandFactory.CreateParameterCommand<ILaminarStorageItem>(
            "Toggle Enabled",
            item => item.IsEnabled = !item.IsEnabled,
            item => item.IsEnabled = !item.IsEnabled,
            ToggleEnabledDataTemplate,
            new ReactiveFunc<ILaminarStorageItem, string>(item => new StringBuilder(4)
                .Append("Click to ")
                .Append(item.IsEffectivelyEnabled ? "disable" : "enable")
                .Append(" this ")
                .Append(item is LaminarStorageFolder ? "folder" : "item").ToString()),
            new ReactiveFunc<ILaminarStorageItem, bool>(item => item.ParentIsEffectivelyEnabled),
            new KeyGesture(Key.E, KeyModifiers.Alt)
        );
    
    [Serialize]
    public ObservableCollection<ILaminarStorageItem> RootFiles { get; set; } = [ new LaminarStorageFolder(Path.Combine(dataManager.Path, "Default")) ];

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
}