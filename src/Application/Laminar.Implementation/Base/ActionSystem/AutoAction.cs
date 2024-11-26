using System;
using Laminar.Contracts.Base.ActionSystem;

namespace Laminar.Implementation.Base.ActionSystem;

public class AutoAction : IUserAction
{
    public required Func<bool> ExecuteAction { get; init; }

    public required Func<bool> UndoAction { get; init; }

    public bool Execute()
        => ExecuteAction();

    public IUserAction GetInverse()
        => new AutoAction { ExecuteAction = UndoAction, UndoAction = ExecuteAction };
}