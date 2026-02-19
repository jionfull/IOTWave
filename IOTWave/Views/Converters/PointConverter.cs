using Avalonia.Data.Converters;
using Avalonia.Media;

namespace IOTWave.Views.Converters;

public class PointConverter : IValueConverter
{
    public static readonly PointConverter Instance = new PointConverter();

    public object? Convert(object? value, System.Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is double x)
        {
            return new Point(x, 0);
        }
        return new Point(0, 0);
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
