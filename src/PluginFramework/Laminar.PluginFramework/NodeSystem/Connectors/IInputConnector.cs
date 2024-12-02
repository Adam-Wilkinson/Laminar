namespace Laminar.PluginFramework.NodeSystem.Connectors;

public interface IInputConnector : IIOConnector
{
    public bool CanConnectTo(IOutputConnector connector);
    
    public bool TryConnectTo(IOutputConnector connector);
    
    public void OnDisconnectedFrom(IOutputConnector connector);
}
