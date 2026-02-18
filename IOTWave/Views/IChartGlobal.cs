using Avalonia.Media;

namespace IotWave.Views;

public interface IChartGlobal
{
    DateTime StartTime { get; }
    DateTime EndTime { get; }
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
