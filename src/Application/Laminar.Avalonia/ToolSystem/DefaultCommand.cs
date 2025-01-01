using System;
using System.Windows.Input;

namespace Laminar.Avalonia.ToolSystem;

public class DefaultCommand : ICommand
{
    public static readonly ICommand Instance = new DefaultCommand();
    
    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
    }

    public event EventHandler? CanExecuteChanged;
}