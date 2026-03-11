using System.ComponentModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace IOTWave.Models;

public partial class CurveData :ObservableObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Legend { get; set; }= String.Empty;

    public Dictionary<string, object> Properties { get; set; } = new();

    private Color _color = Colors.Blue;
    public Color Color 
    { 
        get => _color;
        set
        {
            _color = value;
            OnPropertyChanged(nameof(Color));
            OnPropertyChanged(nameof(Brush));
        }
    }
    
    /// <summary>
    /// 获取曲线颜色的 Brush 对象，用于 UI 绑定
    /// </summary>
    public IBrush Brush => new SolidColorBrush(Color);
    
    public List<TimePoint> Points { get; set; } = new();
    public double MinValue => Points.Any() ? Points.Min(p => p.Value) : 0;
    public double MaxValue => Points.Any() ? Points.Max(p => p.Value) : 0;
    public bool ShowPoints { get; set; } = true;
    public double LineWidth { get; set; } = 1;
    
    /// <summary>
    ///  曲线点绘制间隔
    /// </summary>
    public double PointShowLimit { get; set; } = 0.1;

    private bool _isVisible = true;
    /// <summary>
    /// 是否显示该曲线
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
    }

    [ObservableProperty]
    private TimePoint? _cursorPoint;

    /// <summary>
    /// 相对光标时间偏移（相对于基准时间的字符串表示）
    /// </summary>
    [ObservableProperty]
    private string? _relativeCursorTime;

    /// <summary>
    /// 根据光标时间更新当前值（查找光标时间之前最近的数据点）
    /// </summary>
    public void UpdateCurrentValue(DateTime? cursorTime)
    {
        if (!cursorTime.HasValue || Points.Count == 0)
        {
            CursorPoint = null;
            RelativeCursorTime = null;
            return;
        }

        // 二分查找找到第一个时间大于光标时间的点
        var index = Points.BinarySearch(new TimePoint { Time = cursorTime.Value }, TimePoint.TimeComparer.Instance);

        if (index < 0) index = ~index;

        // index 是第一个大于光标时间的点，所以前一个点就是我们要找的点
        if (index > 0)
        {
            CursorPoint = Points[index - 1];
        }
        else if (index == 0 && Points.Count > 0)
        {
            // 光标时间在所有数据点之前
            CursorPoint = null;
            RelativeCursorTime = null;
        }
    }

    /// <summary>
    /// 设置相对光标时间（由 CurvePanel 调用）
    /// </summary>
    public void SetRelativeCursorTime(DateTime baseTime)
    {
        if (CursorPoint == null)
        {
            RelativeCursorTime = null;
            return;
        }
        var offset = CursorPoint.Time - baseTime;
        RelativeCursorTime = FormatRelativeTime(offset);
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
            return $"{(int)duration.TotalDays}d {duration.Hours}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        else if (duration.TotalHours >= 1)
            return $"{(int)duration.TotalHours}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        else if (duration.TotalMinutes >= 1)
            return $"{(int)duration.TotalMinutes}:{duration.Seconds:D2}";
        else if (duration.TotalSeconds >= 1)
            return $"{duration.TotalSeconds:F1}s";
        else
            return $"{duration.TotalMilliseconds:F0}ms";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

}