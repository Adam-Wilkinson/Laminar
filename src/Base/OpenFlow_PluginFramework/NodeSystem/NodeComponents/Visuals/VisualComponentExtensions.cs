﻿using OpenFlow_PluginFramework.Primitives.TypeDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlow_PluginFramework.NodeSystem.NodeComponents.Visuals
{
    public static class VisualComponentExtensions
    {
        public static T WithFlowInput<T>(this T component, bool HasFlowInput = true) where T : IVisualNodeComponent
        {
            component.SetFlowInput(HasFlowInput);
            return component;
        }

        public static T WithFlowOutput<T>(this T component, bool HasFlowOutput = true) where T : IVisualNodeComponent
        {
            component.SetFlowOutput(HasFlowOutput);
            return component;
        }

        public static TComponent WithValue<TComponent>(this TComponent nodeField, string valueKey, object defaultValue, bool isUserEditable = false) where TComponent : INodeField
        {
            nodeField.AddValue(valueKey, defaultValue, isUserEditable);
            return nodeField;
        }

        public static TComponent WithInput<TComponent>(this TComponent nodeField, object defaultValue) where TComponent : INodeField
            => nodeField.WithValue(INodeField.InputKey, defaultValue, true);

        public static TComponent WithOutput<TComponent>(this TComponent nodeField, object defaultValue) where TComponent : INodeField
            => nodeField.WithValue(INodeField.OutputKey, defaultValue, false);

        public static object GetInput(this INodeField nodeField) => nodeField[INodeField.InputKey];

        public static T GetInput<T>(this INodeField nodeField) => (T)nodeField[INodeField.InputKey];

        public static void SetInput(this INodeField nodeField, object value) => nodeField[INodeField.InputKey] = value;

        public static object GetOutput(this INodeField nodeField) => nodeField[INodeField.OutputKey];

        public static T GetOutput<T>(this INodeField nodeField) => (T)nodeField[INodeField.OutputKey];

        public static void SetOutput(this INodeField nodeField, object value) => nodeField[INodeField.OutputKey] = value;
    }
}