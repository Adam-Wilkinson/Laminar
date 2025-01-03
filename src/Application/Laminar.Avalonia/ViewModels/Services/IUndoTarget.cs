using System.Windows.Input;

namespace Laminar.Avalonia.ViewModels.Services;

public interface IUndoTarget
{
    public ICommand UndoCommand { get; }
}