﻿using Laminar.Contracts.Base;
using Laminar.Contracts.UserData.Settings;
using Laminar.Implementation.Base;
using Laminar.Implementation.UserData.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Implementation.Extensions.ServiceInitializers;

internal static class EnvironmentServices
{
    public static IServiceCollection AddEnvironmentServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ITypeInfoStore, TypeInfoStore>();

        serviceCollection.AddSingleton<IUserPreferenceManager, UserPreferenceManager>();
        
        return serviceCollection;
    }
}
