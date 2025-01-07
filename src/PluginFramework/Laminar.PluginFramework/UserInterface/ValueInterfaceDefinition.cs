using System.ComponentModel;
using System.Runtime.CompilerServices;
using Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

namespace Laminar.PluginFramework.UserInterface;
public interface IValueInterfaceDefinition
{
    public Type? ValueType { get; }

    public bool IsUserEditable { get; set; }

    public IUserInterfaceDefinition? Editor { get; set; }

    public IUserInterfaceDefinition? Viewer { get; set; }

    public IUserInterfaceDefinition? GetCurrentDefinition();
}

public class UserInterface<TInterfaceDefinition, TValue> : ValueInterface<TValue> where TInterfaceDefinition : IUserInterfaceDefinition, new()
{
    public TInterfaceDefinition Definition { get; init; } = new();
}

public class ValueInterface<T> : NamedInterface, INotifyPropertyChanged
{
    private T _field = default!;

    public required T Value
    {
        get => _field;
        set => SetField(ref _field, value);
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class NamedInterface
{
    public string Name { get; init; } = "";
}