using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Laminar.Avalonia.ViewModels;
using Laminar.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Avalonia;
public partial class App : Application
{
    public static readonly IEnumerable<Type> ViewModels = 
        typeof(App).Assembly.GetTypes()
            .Where(x => x is { IsClass: true, IsAbstract: false } && x.IsSubclassOf(typeof(ViewModelBase)));

    public static ServiceProvider Locator { get; private set; }

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

            var collection = new ServiceCollection();
            foreach (var viewModelType in ViewModels)
            {
                collection.AddTransient(viewModelType);
            }
                
            desktop.MainWindow = new MainWindow();

            collection.AddSingleton<IStorageProvider>(_ => desktop.MainWindow.StorageProvider);
            var services = collection.BuildServiceProvider();
            Locator = services;
            desktop.MainWindow.DataContext = services.GetRequiredService<MainWindowViewModel>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}