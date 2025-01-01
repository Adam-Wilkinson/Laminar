using System;
using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Laminar.Avalonia.Converters;

public class NullOrEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            ICollection collection => collection.Count > 0,
            string str => string.IsNullOrWhiteSpace(str),
            _ => value is not null
        };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}