using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

namespace Laminar.Avalonia.ViewModels;
public class FileNavigatorViewModel(
    IStorageProvider storageProvider, 
    IPersistentDataManager dataManager, 
    LaminarCommandFactory commandFactory) : ViewModelBase
{
    private readonly IStorageProvider _storageProvider = storageProvider;

    public LaminarCommandVisual ToggleEnable { get; } =
        commandFactory.CreateCommand<ILaminarStorageItem>(
            "Toggle Enabled",
            item => item.IsEnabled = !item.IsEnabled,
            item => item.IsEnabled = !item.IsEnabled,
            canExecute: item => item.ParentIsEffectivelyEnabled,
            description: "Enable/disable",
            keyGesture: new KeyGesture(Key.E, KeyModifiers.Alt)
        ).WithVisual(commandWithParameter =>
        {
            var newSwitch = new LaminarCommandSwitch
            {
                [!InputElement.IsEnabledProperty] = 
                    new Binding { Source = commandWithParameter.Parameter, Path = nameof(ILaminarStorageItem.ParentIsEffectivelyEnabled), },
                [!LaminarCommandSwitch.IsOnProperty] = 
                    new Binding { Source = commandWithParameter.Parameter, Path = nameof(ILaminarStorageItem.IsEffectivelyEnabled) },
            };

            newSwitch.Tapped += (_, _) => commandWithParameter.Execute();
            
            return newSwitch;
        });
            
    
    [Serialize]
    public ObservableCollection<ILaminarStorageItem> RootFiles { get; set; } = [ new LaminarStorageFolder(Path.Combine(dataManager.Path, "Default")) ];

    public void OpenFilePicker()
    {
        _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { AllowMultiple = false, Title = "Pick a file!"});
    }
}