using System.ComponentModel;

namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class EditableLabel : IUserInterfaceDefinition, INotifyPropertyChanged
{
    public class UITarget : UserInterface<EditableLabel>
    {
    }

    private bool _isBeingEdited;

    public bool IsBeingEdited
    {
        get => _isBeingEdited;
        set
        {
            if (_isBeingEdited != value)
            {
                _isBeingEdited = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBeingEdited)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
