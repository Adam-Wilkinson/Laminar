using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Laminar.Avalonia.Commands;
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
            desktop.MainWindow = new MainWindow();

            var services = new ServiceCollection()
                .AddLaminarServices()
                .AddDescendantsTransient<ViewModelBase>()
                .AddSingleton<LaminarToolFactory>()
                .AddSingleton(desktop.MainWindow.StorageProvider)
                .AddSingleton<TopLevel>(desktop.MainWindow)
                .BuildServiceProvider();
            
            var mainWindowViewModel = services.GetRequiredService<MainWindowViewModel>();
            
            ActivatorUtilities.CreateInstance<ViewModelSerializationHelper>(services)
                .SerializeObservableProperties(mainWindowViewModel,
                    services.GetRequiredService<IPersistentDataManager>().GetDataStore(DataStoreKey.PersistentData));
            
            desktop.MainWindow.DataContext = mainWindowViewModel;
        }

        base.OnFrameworkInitializationCompleted();
    }
}