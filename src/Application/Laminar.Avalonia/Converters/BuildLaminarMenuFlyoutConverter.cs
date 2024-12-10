using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Styling;
using Laminar.Avalonia.ToolSystem;

namespace Laminar.Avalonia.Converters;

public class BuildLaminarMenuFlyoutConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return null;
        
        if (value is not IEnumerable<LaminarToolInstance> laminarTools || parameter is not ControlTheme menuItemTheme) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return new MenuFlyout
        {
            ItemContainerTheme = menuItemTheme,
            ItemsSource = laminarTools,
            FlyoutPresenterClasses = { "ArrowCursor" }
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}