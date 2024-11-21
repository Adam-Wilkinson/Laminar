using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Laminar.Contracts.UserData;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Avalonia.ViewModels;
public abstract class ViewModelBase : ObservableObject
{
    private static readonly IEnumerable<Type> AllViewModels = typeof(App).Assembly.GetTypes()
        .Where(x => x is { IsClass: true, IsAbstract: false } && x.IsSubclassOf(typeof(ViewModelBase)));

    protected T GetPersistentData<T>(IPersistentDataStore dataStore, [CallerMemberName] string propertyName = "")
        where T : notnull
    {
        return (dataStore.GetItem<T>(propertyName).Result ?? default)!;
    }

    protected void SetPersistentData<T>(IPersistentDataStore dataStore, T value,
        [CallerMemberName] string propertyName = "")
        where T : notnull
    {
        OnPropertyChanging(propertyName);
        dataStore.SetItem(propertyName, value);
        OnPropertyChanged(propertyName);
    }
    
    public static IServiceCollection RegisterAll(IServiceCollection services)
    {
        foreach (var viewModel in AllViewModels)
        {
            services.AddTransient(viewModel);
        }
        
        return services;
    }
}
