namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public interface ITypedInterfaceDefinition<out T>
{
    public T DefaultValue { get; }
}

public interface IUserInterfaceDefinition
{
}
