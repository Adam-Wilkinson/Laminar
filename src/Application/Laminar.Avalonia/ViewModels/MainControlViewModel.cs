using CommunityToolkit.Mvvm.ComponentModel;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;

namespace Laminar.Avalonia.ViewModels;
public partial class MainControlViewModel(FileNavigatorViewModel fileNavigator, IPersistentDataManager dataManager) : ViewModelBase
{
    private readonly IPersistentDataStore _dataStore = dataManager.GetDataStore(DataStoreKey.PersistentData)
        .InitializeDefaultValue<double>(nameof(SidebarWidth), 400);
    
    [ObservableProperty] private bool _sidebarExpanded = true;

    public double SidebarWidth
    {
        get => GetPersistentData<double>(_dataStore);
        set => SetPersistentData(_dataStore, value);
    }
    
    public FileNavigatorViewModel FileNavigator { get; } = fileNavigator;
}