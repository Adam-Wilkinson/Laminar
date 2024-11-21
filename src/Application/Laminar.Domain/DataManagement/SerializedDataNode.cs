namespace Laminar.Domain.DataManagement;

public struct SerializedDataNode
{
    public SerializedDataNode(string key, object value)
    {
        Key = key;
        Value = value;
    }

    public SerializedDataNode(string key, Dictionary<string, SerializedDataNode> nodes)
    {
        Key = key;
        Children = nodes;
    }
    
    public string Key { get; }
    
    public object? Value { get; }

    public Dictionary<string, SerializedDataNode>? Children { get; }
}