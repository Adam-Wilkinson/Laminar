using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.Media.TextFormatting;
using Avalonia.Reactive;

namespace BasicFunctionality.Avalonia.UserControls;
public partial class StringEditor : UserControl
{
    private TextPresenter? _entryTextLayout;
    private double _textPresenterMargin;
    
    public StringEditor()
    {
        InitializeComponent();
        //
        // EntryTextbox.TemplateApplied += (_, args) =>
        // {
        //     _entryTextLayout = args.NameScope.Find<TextPresenter>("PART_TextPresenter");
        // };
        //
        // EntryTextbox.Loaded += (_, _) =>
        // {
        //     if (_entryTextLayout is not null)
        //     {
        //         _textPresenterMargin = EntryTextbox.Bounds.Width - _entryTextLayout.Bounds.Width;
        //     }
        //     
        //     UpdateCutoff();
        // };
        //
        // EntryTextbox.TextChanged += (_, _) => { UpdateCutoff(); };
    }

    private void UpdateCutoff()
    {
        if (_entryTextLayout?.TextLayout is null) return;

        Compact.SetCutoff(EntryTextbox, _entryTextLayout.TextLayout.Width + _textPresenterMargin);
    }

    public void InvalidChain()
    {
        MainPanel.InvalidateMeasure();
    }
}
