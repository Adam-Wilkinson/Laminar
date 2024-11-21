using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Laminar.Avalonia.ViewModels;
using Laminar.Avalonia.Views;
using Laminar.Contracts.UserData;
using Laminar.Domain.DataManagement;
using Laminar.Implementation.Extensions.ServiceInitializers;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Avalonia;
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            var collection = new ServiceCollection()
                .AddLaminarServices()
                .AddDescendantsTransient<ViewModelBase>();
                
            desktop.MainWindow = new MainWindow();
            collection.AddSingleton(desktop.MainWindow.StorageProvider);
            var services = collection.BuildServiceProvider();

            var mainWindowViewModel = services.GetRequiredService<MainWindowViewModel>();
            ActivatorUtilities.CreateInstance<ViewModelSerializationHelper>(services)
                .SerializeObservableProperties(mainWindowViewModel,
                    services.GetRequiredService<IPersistentDataManager>().GetDataStore(DataStoreKey.PersistentData));
            
            desktop.MainWindow.DataContext = mainWindowViewModel;
        }

        base.OnFrameworkInitializationCompleted();
    }
}