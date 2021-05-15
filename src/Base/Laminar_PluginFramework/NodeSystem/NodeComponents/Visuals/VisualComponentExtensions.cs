﻿using Laminar_PluginFramework.Primitives;
using Laminar_PluginFramework.Primitives.TypeDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laminar_PluginFramework.NodeSystem.NodeComponents.Visuals
{
    public static class VisualComponentExtensions
    {
        public static T WithFlowInput<T>(this T component, bool HasFlowInput = true) where T : IVisualNodeComponent
        {
            component.FlowInput.Exists = HasFlowInput;
            return component;
        }

        public static T WithFlowOutput<T>(this T component, bool HasFlowOutput = true) where T : IVisualNodeComponent
        {
            component.FlowOutput.Exists = HasFlowOutput;
            return component;
        }

        public static TComponent WithValue<TComponent>(this TComponent nodeField, object valueKey, object defaultValue, bool isUserEditable = false) where TComponent : INodeField
        {
            nodeField.AddValue(valueKey, defaultValue, isUserEditable);
            return nodeField;
        }
        public static TComponent WithValue<TComponent>(this TComponent nodeField, object valueKey, ILaminarValue value) where TComponent : INodeField
        {
            nodeField.AddValue(valueKey, value);
            return nodeField;
        }

        public static TComponent WithInput<TComponent>(this TComponent nodeField, object defaultValue, bool isUserEditable = true) where TComponent : INodeField
            => nodeField.WithValue(INodeField.InputKey, defaultValue, isUserEditable);

        public static TComponent WithOutput<TComponent>(this TComponent nodeField, object defaultValue, bool isUserEditable = false) where TComponent : INodeField
            => nodeField.WithValue(INodeField.OutputKey, defaultValue, isUserEditable);

        public static INodeField WithValue<T>(this INodeField nodeField, object valueKey, bool isUserEditable)
        {
            nodeField.AddValue<T>(valueKey, isUserEditable);
            return nodeField;
        }

        public static INodeField WithInput<T>(this INodeField nodeField, bool isUserEditable = true)
        {
            nodeField.AddValue<T>(INodeField.InputKey, isUserEditable);
            return nodeField;
        }

        public static INodeField WithOutput<T>(this INodeField nodeField, bool isUserEditable = false)
        {
            nodeField.AddValue<T>(INodeField.OutputKey, isUserEditable);
            return nodeField;
        }

        public static object GetInput(this INodeField nodeField) => nodeField[INodeField.InputKey];

        public static T GetInput<T>(this INodeField nodeField) => (T)nodeField[INodeField.InputKey];

        public static void SetInput(this INodeField nodeField, object value) => nodeField[INodeField.InputKey] = value;

        public static object GetOutput(this INodeField nodeField) => nodeField[INodeField.OutputKey];

        public static T GetOutput<T>(this INodeField nodeField) => (T)nodeField[INodeField.OutputKey];

        public static void SetOutput(this INodeField nodeField, object value) => nodeField[INodeField.OutputKey] = value;
    }
}
