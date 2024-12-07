using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Laminar.Avalonia.Controls;

public partial class EditableLabel : UserControl
{
    private bool _isEditing;
    
    public static readonly DirectProperty<EditableLabel, bool> IsBeingEditedProperty =
        AvaloniaProperty.RegisterDirect<EditableLabel, bool>(nameof(IsBeingEdited), x => x.IsBeingEdited);
    
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
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public bool IsBeingEdited
    {
        get => _isEditing;
        set => SetAndRaise(IsBeingEditedProperty, ref _isEditing, value);
    }

    private void IsBeingEditedChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (args.GetNewValue<bool>())
        {
            Editor.Text = Text;
            Display.IsVisible = false;
            Editor.IsVisible = true;
            Editor.Focus();
            Editor.SelectAll();
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
