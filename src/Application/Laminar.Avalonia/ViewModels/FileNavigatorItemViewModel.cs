using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using Laminar.Avalonia.ToolSystem;
using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData.FileNavigation;
using Laminar.Domain.Notification;
using Laminar.Implementation.UserData.FileNavigation;
using Laminar.Implementation.UserData.FileNavigation.UserActions;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Avalonia.ViewModels;

public class FileNavigatorItemViewModel : ViewModelBase
{
    private readonly IUserActionManager _actionManager;
    private readonly ILaminarStorageItemFactory _storageFactory;
    
    public FileNavigatorItemViewModel(ILaminarStorageItem coreItem, IUserActionManager actionManager, ILaminarStorageItemFactory storageFactory)
    {
        _actionManager = actionManager;
        _storageFactory = storageFactory;
        CoreItem = coreItem;
        Children = coreItem is ILaminarStorageFolder coreFolder
            ? new MappedObservableCollection<ILaminarStorageItem, FileNavigatorItemViewModel>(coreFolder.Contents,
                item => new FileNavigatorItemViewModel(item, _actionManager, _storageFactory))
            : null;

        CoreItem.ParentFolder.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ILaminarStorageItem.IsEffectivelyEnabled))
                OnPropertyChanged(nameof(ParentFolderIsEffectivelyEnabled));
        };

        CoreItem.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ILaminarStorageItem.IsEnabled))
                OnPropertyChanged(nameof(ToggleEnabledString));
            if (e.PropertyName == nameof(ILaminarStorageItem.Name))
                OnPropertyChanged(nameof(Name));
        };
    }

    public string Name
    {
        get => CoreItem.Name;
        set => _actionManager.ExecuteAction(new RenameStorageItemAction(value, CoreItem));
    }
    
    public ILaminarStorageItem CoreItem { get; }

    public string ItemTypeName => CoreItem switch
    {
        LaminarStorageFolder => "folder",
        LaminarStorageFile => "script",
        _ => "item"
    };
    
    public string ToggleEnabledString => $"{(CoreItem.IsEnabled ? "Disable" : "Enable")} this {ItemTypeName}";
    
    public bool ParentFolderIsEffectivelyEnabled => CoreItem.ParentFolder.IsEffectivelyEnabled;

    public IReadOnlyObservableCollection<FileNavigatorItemViewModel>? Children { get; }

    public object LaminarToolTarget => CoreItem;

    public void AddFolder(object? _)
    {
        if (CoreItem is not ILaminarStorageFolder folder) return;
        _actionManager.ExecuteAction(new AddDefaultStorageItemAction<ILaminarStorageFolder>(folder, _storageFactory));
    }

    public bool CanAddFolder(object? _) => CoreItem is ILaminarStorageFolder;

    public void AddScript()
    {
        if (CoreItem is not ILaminarStorageFolder folder) return;
        _actionManager.ExecuteAction(new AddDefaultStorageItemAction<ILaminarStorageItem>(folder, _storageFactory));
    }

    public void Rename()
    {
        CoreItem.NeedsName = true;
    }

    public void Delete()
    {
        _actionManager.ExecuteAction(new DeleteStorageItemAction<ILaminarStorageItem>(CoreItem));
    }

    public void ToggleEnabled(object? _) => CoreItem.IsEnabled = !CoreItem.IsEnabled;

    [DependsOn(nameof(ParentFolderIsEffectivelyEnabled))]
    public bool CanToggleEnabled(object? _) => CoreItem.ParentFolder.IsEffectivelyEnabled;
}

public class FileNavigatorItemViewModelSerializer(ILaminarStorageItemFactory storageItemFactory) : TypeSerializer<FileNavigatorItemViewModel, string>
{
    protected override string SerializeTyped(FileNavigatorItemViewModel toSerialize)
        => toSerialize.CoreItem.Path;

    protected override FileNavigatorItemViewModel DeSerializeTyped(string serialized, object? deserializationContext = null)
    {
        if (deserializationContext is not FileNavigatorViewModel fileNavigator)
        {
            throw new ArgumentException("DeserializationContext must be of type FileNavigatorViewModel", nameof(deserializationContext));
        }

        return fileNavigator.NewItem(storageItemFactory.FromPath(serialized));
    }
}