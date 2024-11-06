using Laminar.Contracts.UserData;
using Laminar.Implementation.UserData;
using Laminar.PluginFramework.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Implementation.Extensions.ServiceInitializers;

public static class LaminarServices
{
    public static IServiceCollection AddLaminarServices(this IServiceCollection services)
    {
        services.AddSingleton<IPersistentDataManager, PersistentDataManager>();
        services.AddSingleton<ISerializer, Serializer>();
        services.AddSingleton<ILaminarFileManager, LaminarFileManager>();
        return services;
    }
}