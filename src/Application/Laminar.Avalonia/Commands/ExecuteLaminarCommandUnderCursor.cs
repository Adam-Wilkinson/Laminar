using System;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;

namespace Laminar.Avalonia.Commands;

public class ExecuteLaminarCommandUnderCursor(LaminarCommand laminarCommand, LaminarCommandFactory factory) : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return TryGetCurrentParameter(out _);
    }

    public void Execute(object? parameter)
    {
        if (!TryGetCurrentParameter(out var currentParameter)) return;
        laminarCommand.Execute(currentParameter);
    }

    public event EventHandler? CanExecuteChanged;

    private bool TryGetCurrentParameter(out object? parameter)
    {
        if (factory.VisualUnderCursor is null)
        {
            parameter = default;
            return false;
        }

        foreach (var visual in factory.VisualUnderCursor.GetVisualAncestors())
        {
            switch (visual)
            {
                case object obj when laminarCommand.CanExecute(obj):
                    parameter = visual;
                    return true;
                case ContentControl contentControl when laminarCommand.CanExecute(contentControl.Content):
                    parameter = contentControl.Content;
                    return true;
                case ContentPresenter contentPresenter when laminarCommand.CanExecute(contentPresenter.Content):
                    parameter = contentPresenter.Content;
                    return true;
            }
        }

        parameter = default;
        return false;
    }
}