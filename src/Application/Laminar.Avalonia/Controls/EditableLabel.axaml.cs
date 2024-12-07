using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Laminar.Avalonia.Controls;

public partial class EditableLabel : UserControl
{
    public static readonly StyledProperty<bool> IsBeingEditedProperty = AvaloniaProperty.Register<EditableLabel, bool>(nameof(IsBeingEdited));
    
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<EditableLabel, string>(nameof(Text));
    
    static EditableLabel()
    {
        IsBeingEditedProperty.Changed.AddClassHandler<EditableLabel>((label, args) => label.IsBeingEditedChanged(args));
    }

    public EditableLabel()
    {
        InitializeComponent();

        Display.DoubleTapped += (o, e) => IsBeingEdited = true;
        Display[!TextBlock.TextProperty] = this[!TextProperty];
        
        Editor.KeyDown += Entry_KeyDown;

        Editor.GotFocus += (sender, args) =>
        {
            Debug.WriteLine("Got focus");
        };

        Editor.AttachedToVisualTree += (sender, args) =>
        {
            if (IsBeingEdited)
            {
                Editor.Focus();
            }
        };

        Editor.LostFocus += (sender, args) =>
        {
            Debug.WriteLine("Lost focus");
        };
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public bool IsBeingEdited
    {
        get => GetValue(IsBeingEditedProperty);
        set => SetValue(IsBeingEditedProperty, value);
    }

    private void IsBeingEditedChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.GetNewValue<bool>())
        {
            Editor.Text = Text;
            Display.IsVisible = false;
            Editor.IsVisible = true;
            Editor.SelectAll();
            Editor.Focus();
        }
        else
        {
            Display.IsVisible = true;
            Editor.IsVisible = false;
        }
    }

    private void Entry_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Text = Editor.Text ?? string.Empty;
            IsBeingEdited = false;
        }

        if (e.Key == Key.Escape)
        {
            IsBeingEdited = false;
        }
    }
}
