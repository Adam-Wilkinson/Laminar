namespace Laminar.Avalonia.ViewModels.Services;

public interface IUndoTarget
{
    public void Undo();

    public bool CanUndo { get; }
}