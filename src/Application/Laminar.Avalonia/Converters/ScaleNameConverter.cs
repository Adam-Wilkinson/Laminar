using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Laminar.Avalonia.Converters;

public class ScaleNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double scaleNumber || targetType != typeof(string))
        {
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        return scaleNumber switch
        {
            <= 1.0 => "Small",
            > 1.0 and <= 1.2 => "Medium",
            > 1.2 and <= 1.4 => "Large",
            _ => "Huge",
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string scaleName || targetType != typeof(double))
        {
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        return scaleName switch
        {
            "Small" => 0.8,
            "Medium" => 1.0,
            "Large" => 1.3,
            "Huge" => 1.5,
            _ => new BindingNotification(new InvalidDataException(), BindingErrorType.DataValidationError),
        };
    }
}