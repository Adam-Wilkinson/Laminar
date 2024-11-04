using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Implementation.Extensions.ServiceInitializers;

public static class DescendantServices
{
    public static IServiceCollection AddDescendantsTransient<T>(this IServiceCollection collection)
    { 
        foreach (var descendantType in DescendantTypes<T>())
        {
            collection.AddTransient(descendantType);
        }

        return collection;
    }

    public static IServiceCollection AddDescendantsScoped<T>(this IServiceCollection collection)
    {
        foreach (var descendantType in DescendantTypes<T>())
        {
            collection.AddScoped(descendantType);
        }

        return collection;
    }

    public static IServiceCollection AddDescendantsSingleton<T>(this IServiceCollection collection)
    {
        foreach (var descendantType in DescendantTypes<T>())
        {
            collection.AddSingleton(descendantType);
        }
        
        return collection;
    }

    private static IEnumerable<Type> DescendantTypes<T>()
        => typeof(T).Assembly.GetTypes()
            .Where(x => x is { IsClass: true, IsAbstract: false } && x.IsSubclassOf(typeof(T)));
}