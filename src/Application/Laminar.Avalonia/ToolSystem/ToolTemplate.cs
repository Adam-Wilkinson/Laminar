using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Laminar.Avalonia.ToolSystem;

public class ToolboxTemplate : ToolTemplate
{
    [Content] public AvaloniaList<ToolTemplate> ChildrenContent => ChildTools;
}

public class ToolTemplate : AvaloniaObject, ITemplate<object?, ToolInstance>, IEnumerable<ToolTemplate>
{
    public static readonly StyledProperty<KeyGesture?> GestureProperty = AvaloniaProperty.Register<ToolTemplate, KeyGesture?>(nameof(Gesture));

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty = AvaloniaProperty.Register<ToolTemplate, IDataTemplate?>(nameof(IconTemplate));
    
    public static readonly StyledProperty<IBinding?> CommandBindingProperty = AvaloniaProperty.Register<ToolTemplate, IBinding?>(nameof(CommandBinding));
    
    public static readonly StyledProperty<IBinding?> DescriptionBindingProperty = AvaloniaProperty.Register<ToolTemplate, IBinding?>(nameof(DescriptionBinding));

    private Geometry? _defaultIconGeometry; 
    
    public string Name { get; set; } = string.Empty;

    public KeyGesture? Gesture
    {
        get => GetValue(GestureProperty);
        set => SetValue(GestureProperty, value);
    }
    
    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }
    
    [AssignBinding]
    public IBinding? DescriptionBinding
    {
        get => GetValue(DescriptionBindingProperty);
        set => SetValue(DescriptionBindingProperty, value);
    }
    
    [AssignBinding]
    public IBinding? CommandBinding
    {
        get => GetValue(CommandBindingProperty);
        set => SetValue(CommandBindingProperty, value);
    }

    public Geometry? DefaultIconGeometry
    {
        get => _defaultIconGeometry;
        set
        {
            _defaultIconGeometry = value;
            if (value is not null && IconTemplate is null)
            {
                IconTemplate = new FuncDataTemplate(_ => true, (_, _) => new GeometryIcon { Data = value });   
            }
        }
    }
    
    public AvaloniaList<ToolTemplate> ChildTools { get; } = [];

    [Content]
    [TemplateContent(TemplateResultType = typeof(ToolInstance))]
    public object? Content { get; set; }

    public ToolInstance Build(object? param)
    {
        var newTool = TemplateContent.Load<ToolInstance>(Content)?.Result ?? new ToolInstance();
        newTool.DataContext = param;
        newTool.ToolTemplate = this;
        if (newTool.Command == DefaultCommand.Instance && CommandBinding is not null)
        {
            newTool[!ToolInstance.CommandProperty] = CommandBinding;
        }

        if (string.IsNullOrEmpty(newTool.Description) && DescriptionBinding is not null)
        {
            newTool[!ToolInstance.DescriptionProperty] = DescriptionBinding;
        }
        return newTool;
    }

    public IEnumerator<ToolTemplate> GetEnumerator() => ChildTools.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [DataType] 
    public Type? DataType { get; set; }
}
