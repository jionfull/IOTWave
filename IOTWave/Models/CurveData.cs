using System.ComponentModel;

namespace IotWave.Models;

public class CurveData : INotifyPropertyChanged
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Legend { get; set; }= String.Empty;
    public Color Color { get; set; } = Colors.Blue;
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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}