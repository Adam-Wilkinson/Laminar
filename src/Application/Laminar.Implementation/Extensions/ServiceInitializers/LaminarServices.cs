using Laminar.Contracts.Base.ActionSystem;
using Laminar.Contracts.UserData;
using Laminar.Contracts.UserData.FileNavigation;
using Laminar.Implementation.Base.ActionSystem;
using Laminar.Implementation.UserData;
using Laminar.Implementation.UserData.FileNavigation;
using Laminar.PluginFramework.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Laminar.Implementation.Extensions.ServiceInitializers;

public static class LaminarServices
{
    public static IServiceCollection AddLaminarServices(this IServiceCollection services) => services
            .AddSingleton<IPersistentDataManager, PersistentDataManager>()
            .AddSingleton<ISerializer, Serializer>()
            .AddSingleton<IUserActionManager, UserActionManager>()
            .AddSingleton<ILaminarStorageItemFactory, LaminarStorageItemFactory>();
}