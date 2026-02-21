using System.Globalization;
using Avalonia.Data.Converters;

namespace IOTWave.Views.Converters;

/// <summary>
/// 将时间转换为相对于基准时间的字符串表示
/// </summary>
public class RelativeTimeConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return "--";

        if (values[0] is not DateTime time)
            return "--";

        // 如果没有基准时间或基准时间为MinValue，显示绝对时间
        if (values[1] is not DateTime baseTime || baseTime == DateTime.MinValue)
            return time.ToString("HH:mm:ss.fff");

        var offset = time - baseTime;
        return FormatRelativeTime(offset);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static string FormatRelativeTime(TimeSpan offset)
    {
        if (offset.TotalSeconds < 0)
        {
            return $"-{FormatPositiveTime(offset.Duration())}";
        }
        return FormatPositiveTime(offset);
    }

    private static string FormatPositiveTime(TimeSpan duration)
    {
        if (duration.TotalDays >= 1)
        {
            return $"{(int)duration.TotalDays}d {duration.Hours}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        }
        else if (duration.TotalHours >= 1)
        {
            return $"{(int)duration.TotalHours}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        }
        else if (duration.TotalMinutes >= 1)
        {
            return $"{(int)duration.TotalMinutes}:{duration.Seconds:D2}";
        }
        else if (duration.TotalSeconds >= 1)
        {
            return $"{duration.TotalSeconds:F1}s";
        }
        else
        {
            return $"{duration.TotalMilliseconds:F0}ms";
        }
    }
}
