﻿using Laminar_Core.NodeSystem.Nodes;
using Laminar_Core.Scripting;
using Laminar_Core.Scripting.Advanced.Editing;
using Laminar_Core.Scripting.Advanced.Editing.Connection;
using Laminar_Core.Scripting.Advanced.Instancing;
using Laminar_PluginFramework.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Laminar_Core.Scripting.Advanced
{
    public interface IAdvancedScript
    {
        public IAdvancedScriptEditor Editor { get; }

        public bool IsBeingEdited { get; set; }

        public IObservableValue<string> Name { get; }

        IAdvancedScriptInputs Inputs { get; }

        public IAdvancedScriptInstance CreateInstance();
    }
}