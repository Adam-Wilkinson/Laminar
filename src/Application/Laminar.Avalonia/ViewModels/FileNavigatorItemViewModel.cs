using System;
using System.Collections.ObjectModel;
using Laminar.Avalonia.ToolSystem;
using Laminar.Contracts.UserData.FileNavigation;
using Laminar.Domain.Notification;
using Laminar.Implementation.UserData.FileNavigation;
using Laminar.PluginFramework.Serialization;

namespace Laminar.Avalonia.ViewModels;

public class FileNavigatorItemViewModel(ILaminarStorageItem coreItem, FileNavigatorViewModel fileNavigator) : ViewModelBase, ILaminarToolTargetRedirect
{
    public ILaminarStorageItem CoreItem => coreItem;

    public string ItemTypeName => CoreItem switch
    {
        LaminarStorageFolder => "folder",
        LaminarStorageFile => "script",
        _ => "item"
    };

    public ObservableCollection<LaminarTool> QuickAccess => CoreItem switch
    {
        LaminarStorageFile => fileNavigator.FileQuickAssess,
        LaminarStorageFolder => fileNavigator.FolderQuickAccess,
        _ => []
    };
    
    public IReadOnlyObservableCollection<FileNavigatorItemViewModel>? Children { get; } =
        coreItem is ILaminarStorageFolder coreFolder
            ? new MappedObservableCollection<ILaminarStorageItem, FileNavigatorItemViewModel>(coreFolder.Contents,
                item => new FileNavigatorItemViewModel(item, fileNavigator))
            : null;

    public object LaminarToolTarget => CoreItem;
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
        
        return new FileNavigatorItemViewModel(storageItemFactory.FromPath(serialized), fileNavigator);
    }
}