using System.ComponentModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace IOTWave.Models;

public partial class CurveData :ObservableObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Legend { get; set; }= String.Empty;
    
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
    /// 根据光标时间更新当前值（查找光标时间之前最近的数据点）
    /// </summary>
    public void UpdateCurrentValue(DateTime? cursorTime)
    {
        if (!cursorTime.HasValue || Points.Count == 0)
        {
            CursorPoint = null;
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
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

}