using Laminar.PluginFramework.UserInterface;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace Laminar.Contracts.Base.UserInterface;

public interface IDataInterfaceFactory
{
    public void RegisterInterface<TInterfaceDefinition, TValue, TInterface>()
        where TInterfaceDefinition : IUserInterfaceDefinition, new()
        where TValue : notnull;

    public IDataInterface<TFrontend> GetDataInterface<TFrontend>(IInterfaceData interfaceData)
        where TFrontend : class, new();
}