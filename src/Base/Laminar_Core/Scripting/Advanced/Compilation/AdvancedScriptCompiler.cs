﻿using Laminar_Core.NodeSystem.Nodes;
using Laminar_Core.NodeSystem;
using Laminar_PluginFramework.NodeSystem.Nodes;
using Laminar_PluginFramework.Primitives;
using System.Collections.Generic;
using System.Linq;
using Laminar_Core.Scripting.Advanced.Editing;
using Laminar_Core.Primitives.LaminarValue;
using Laminar_Core.NodeSystem.NodeComponents.Visuals;
using Laminar_Core.Scripting.Advanced.Editing.Connection;
using Laminar_PluginFramework.NodeSystem.NodeComponents.Visuals;
using System;

namespace Laminar_Core.Scripting.Advanced.Compilation
{
    public class AdvancedScriptCompiler : IAdvancedScriptCompiler
    {
        private readonly ILaminarValueFactory _valueFactory;
        private readonly IObjectFactory _factory;

        public AdvancedScriptCompiler(IObjectFactory factory, ILaminarValueFactory valueFactory)
        {
            _valueFactory = valueFactory;
            _factory = factory;
        }

        public Dictionary<INodeContainer, ILaminarValue> Inputs { get; private set; }

        public ICompiledScript Compile(IAdvancedScript script)
        {
            ICompiledScript compiledScript = _factory.CreateInstance<ICompiledScript>();
            compiledScript.OriginalScript = script;
            Inputs = new();

            foreach (INodeContainer scriptInput in script.Inputs.InputNodes)
            {
                ILaminarValue myInputValue = _valueFactory.Get(((InputNodeContainer<InputNode>)scriptInput).GetValue(null), true);
                myInputValue.Name = scriptInput.CoreNode.GetNameLabel().LabelText.Value;
                compiledScript.Inputs.Add(myInputValue);
                Inputs.Add(scriptInput, myInputValue);
            }

            foreach (INodeContainer triggerNode in script.Editor.TriggerNodes)
            {
                if (triggerNode.Name.OutputConnector.ExclusiveConnection is not null)
                {
                    CompiledNodeWrapper wrappedTrigger = new(triggerNode, this);
                    compiledScript.AllTriggerNodes.Add(wrappedTrigger);
                    wrappedTrigger.flowOutChains.Add(new CompiledNodeChain(triggerNode.Name, wrappedTrigger.CoreNode.GetNameLabel().FlowOutput, this));
                }
            }

            return compiledScript;
        }
    }
}
