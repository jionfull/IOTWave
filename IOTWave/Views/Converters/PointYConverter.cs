using Avalonia.Data.Converters;
using Avalonia.Media;

namespace IOTWave.Views.Converters;

public class PointYConverter : IValueConverter
{
    public static readonly PointYConverter Instance = new PointYConverter();

    public object? Convert(object? value, System.Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is double x)
        {
            return new Point(x, 10000);
        }
        return new Point(0, 10000);
    }

    public object? ConvertBack(object? value, System.Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is Point point)
        {
            return point.X;
        }
        return 0.0;
    }
}
