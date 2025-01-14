﻿using System.ComponentModel;

namespace Laminar.PluginFramework.UserInterface.UserInterfaceDefinitions;

public class EditableLabel : IUserInterfaceDefinition, INotifyPropertyChanged
{
    public static readonly UITarget DesignInstance = new() { Name = "Default Name", Value = "Default Value" };
    
    public class UITarget : InterfaceData<EditableLabel, string>
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
