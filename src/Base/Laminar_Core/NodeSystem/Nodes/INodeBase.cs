﻿using Laminar_Core.NodeSystem.NodeComponents.Visuals;
using Laminar_Core.Primitives;
using Laminar_Core.Scripts;
using Laminar_PluginFramework.NodeSystem.NodeComponents.Visuals;
using Laminar_PluginFramework.NodeSystem.Nodes;
using Laminar_PluginFramework.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Laminar_Core.NodeSystem.Nodes
{
    public interface INodeContainer
    {
        public static Dictionary<INode, INodeContainer> NodeBases { get; } = new();

        IObservableValue<bool> ErrorState { get; }

        IVisualNodeComponentContainer Name { get; }

        IEditableNodeLabel NameLabel { get; }

        INotifyCollectionChanged Fields { get; }

        IPoint Location { get; }

        void MakeLive();

        void Update(IAdvancedScriptInstance instance);

        IVisualNodeComponentContainer FlowOutContainer { get; }

        INodeContainer DuplicateNode();
    }
}