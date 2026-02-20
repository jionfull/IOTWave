using Avalonia.Media;

namespace IOTWave.Views;

public interface IChartGlobal
{
    DateTime StartTime { get; }
    DateTime EndTime { get; }
    
    /// <summary>
    /// 是否使用相对时间模式
    /// </summary>
    bool UseRelativeTime { get; }
    
    /// <summary>
    /// 相对时间的基准时间点（显示为 0s）
    /// </summary>
    DateTime RelativeTimeBase { get; }

    /// <summary>
    /// 相对时间基准的显示标签（如"基准"、"动作时间"等）
    /// </summary>
    string RelativeTimeBaseLabel { get; }
    
    double TimeToX(DateTime time);


    DateTime XToTime(double d);


    
    DateTime GetVisibleEndTime();


    DateTime GetVisibleStartTime();
    List<long> CalculateTickIntervals();

    double GetPixelsPerTick();
    double LeftPadding { get; }
    double RightPadding { get; }

    double BaseInterval { get; }

    IBrush? GridBrush { get; }
    double GridThickness { get; }
    IBrush? LabelBrush { get; }
    IBrush? SeparatorBrush { get; }
    public DateTime? CursorTime { get; }

    event Action InvalidateRequestedEvent;
    event Action ResetYViewRequestedEvent;
}
