using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Avalonia.ViewModels;
public abstract class ViewModelBase : ObservableObject
{
    private static readonly IEnumerable<Type> AllViewModels = typeof(App).Assembly.GetTypes()
        .Where(x => x is { IsClass: true, IsAbstract: false } && x.IsSubclassOf(typeof(ViewModelBase)));

    public static IServiceCollection RegisterAll(IServiceCollection services)
    {
        foreach (var viewModel in AllViewModels)
        {
            services.AddTransient(viewModel);
        }
        
        return services;
    }
}
