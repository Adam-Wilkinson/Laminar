﻿using System;
using System.Collections.Specialized;
using Laminar_PluginFramework.NodeSystem.NodeComponents.Collections;
using Laminar_PluginFramework.NodeSystem.Nodes;
using Laminar_PluginFramework.Primitives;
using Laminar_PluginFramework;
using Laminar_Core.Scripting.Advanced.Editing.Connection;
using Laminar_Core.NodeSystem.NodeComponents.Visuals;
using Laminar_Core.Primitives.ObservableCollectionMapper;
using Laminar_PluginFramework.NodeSystem.NodeComponents.Visuals;
using Laminar_Core.Primitives;
using System.Collections.Generic;
using Laminar_Core.Scripting;
using System.Collections;
using Laminar_Core.Scripting.Advanced.Instancing;

namespace Laminar_Core.NodeSystem.Nodes.NodeTypes
{
    public class NodeContainer<T> 
        : INodeContainer where T : INode, new()
    {
        private T _baseNode;
        private bool _isLive = false;
        private bool _isUpdating = true;
        private readonly IObjectFactory _factory;

        public NodeContainer(NodeDependencyAggregate dependencies)
        {
            (Location, ErrorState, FieldList, _factory) = dependencies;
            Name = _factory.GetImplementation<IVisualNodeComponentContainer>();
            Fields = ObservableCollectionMapper<IVisualNodeComponent, IVisualNodeComponentContainer>.Create(FieldList.VisualNodeComponentsObservable, _factory);

            Fields.CollectionChanged += Fields_CollectionChanged;
        }

        public virtual T BaseNode
        {
            get
            {
                _baseNode ??= _factory.CreateInstance<T>();

                return _baseNode;
            }
            set
            {
                INodeContainer.NodeBases.Remove(value);
                INodeContainer.NodeBases.Add(value, this);

                _baseNode = value;
                _baseNode.GetNameLabel().ParentNode = _baseNode;
                Name.Child = _baseNode.GetNameLabel();


                FieldList.ParentNode = _baseNode;
                FieldList.RemoveAll();
                FieldList.AddRange(BaseNode.Fields);
            }
        }

        public virtual bool IsLive
        {
            get => _isLive;
            set
            {
                _isLive = value;
                _isUpdating = !value;
            }
        }

        protected INodeComponentList FieldList { get; }

        public IVisualNodeComponentContainer Name { get; }

        public IObservableValue<bool> ErrorState { get; }

        public IPoint Location { get; }

        public INotifyCollectionChanged Fields { get; }

        public bool HasFields => FieldList.VisualComponentList.Count > 0;

        public INode CoreNode => BaseNode;

        public virtual INodeContainer DuplicateNode() => new NodeFactory(_factory).Get(BaseNode.Clone());

        public void Update(IAdvancedScriptInstance instance)
        {
            if (_isUpdating)
            {
                return;
            }

            _isUpdating = true;

            foreach (IVisualNodeComponentContainer component in (IList)Fields)
            {
                component.InputConnector.Activate(instance, PropagationDirection.Backwards);
            }

            SafeUpdate(instance);

            foreach (IVisualNodeComponentContainer component in (IList)Fields)
            {
                component.OutputConnector.Activate(instance, PropagationDirection.Forwards);
            }

            BaseNode.GetNameLabel().FlowOutput.Activate();

            _isUpdating = false;
        }

        public virtual void SetFieldValue(IAdvancedScriptInstance instance, INodeField containingField, ILaminarValue laminarValue, object value)
        {
            laminarValue.Value = value;
        }

        public virtual object GetFieldValue(IAdvancedScriptInstance instance, INodeField containingField, ILaminarValue laminarValue)
        {
            return laminarValue.Value;
        }

        protected virtual void SafeUpdate(IAdvancedScriptInstance instance)
        {

        }

        protected IVisualNodeComponentContainer GetContainer(IVisualNodeComponent component)
        {
            if (component == BaseNode.GetNameLabel())
            {
                return Name;
            }

            int index = FieldList.VisualComponentList.IndexOf(component);
            if (index is -1)
            {
                return null;
            }

            return ((IList<IVisualNodeComponentContainer>)Fields)[index];
        }

        public bool CanGetCoreNodeOfType<TRequest>(out TRequest coreNode)
        {
            if (BaseNode is TRequest typedBaseNode)
            {
                coreNode = typedBaseNode;
                return true;
            }

            coreNode = default;
            return false;
        }

        private void Fields_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int index = 0;
            foreach (IVisualNodeComponentContainer container in (IList)Fields)
            {
                container.Child.IndexInParent = index;
            }
        }

        public INode GetCoreNodeInstance() => BaseNode.Clone();
    }
}