using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Transformation;
using Avalonia.Reactive;
using Laminar.Avalonia.ViewModels;

namespace Laminar.Avalonia.Views;
public partial class MainWindow : Window
{
    public static readonly StyledProperty<TransformOperations> HiddenSettingsRenderTransformProperty = AvaloniaProperty.Register<MainWindow, TransformOperations>(nameof(HiddenSettingsRenderTransform));

    public TransformOperations HiddenSettingsRenderTransform
    {
        get => GetValue(HiddenSettingsRenderTransformProperty);
        set => SetValue(HiddenSettingsRenderTransformProperty, value);
    }

    public MainWindow()
    {
        InitializeComponent();

        SettingsMenu.GetObservable(BoundsProperty).Subscribe(new AnonymousObserver<Rect>(SettingsBoundsChanged));
    }

    private void SettingsBoundsChanged(Rect newBounds)
    {
        var operations = TransformOperations.CreateBuilder(1);
        operations.AppendTranslate(0, -newBounds.Height);
        HiddenSettingsRenderTransform = operations.Build();
    }
}