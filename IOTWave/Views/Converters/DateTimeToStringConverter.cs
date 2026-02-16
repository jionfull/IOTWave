using Avalonia.Data.Converters;

namespace IotWave.Views.Converters;

public class DateTimeToStringConverter : IValueConverter
{
    public static readonly DateTimeToStringConverter Instance = new DateTimeToStringConverter();

    public object? Convert(object? value, System.Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss.fff");
        }
        return string.Empty;
    }

    public object? ConvertBack(object? value, System.Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string str && DateTime.TryParse(str, out var dateTime))
        {
            return dateTime;
        }
        return DateTime.Now;
    }
}
