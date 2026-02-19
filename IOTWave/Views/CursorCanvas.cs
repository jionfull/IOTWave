using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Media;
using IOTWave.Models;

namespace IOTWave.Views;

/// <summary>
/// 独立的光标画布控件，用于绘制 TimeMarker、TimeRangeMarker 和 Cursor
/// </summary>
public class CursorCanvas : Control
{
    public static readonly StyledProperty<ObservableCollection<TimeMarker>?> TimeMarkersProperty =
        AvaloniaProperty.Register<CursorCanvas, ObservableCollection<TimeMarker>?>(
            nameof(TimeMarkers));

    public static readonly StyledProperty<ObservableCollection<TimeRangeMarker>?> TimeRangeMarkersProperty =
        AvaloniaProperty.Register<CursorCanvas, ObservableCollection<TimeRangeMarker>?>(
            nameof(TimeRangeMarkers));

    public static readonly StyledProperty<IChartGlobal?> ChartGlobalProperty =
        AvaloniaProperty.Register<CursorCanvas, IChartGlobal?>(
            nameof(ChartGlobal));

    public static readonly StyledProperty<double> LeftPaddingProperty =
        AvaloniaProperty.Register<CursorCanvas, double>(
            nameof(LeftPadding), 36);

    public static readonly StyledProperty<double> RightPaddingProperty =
        AvaloniaProperty.Register<CursorCanvas, double>(
            nameof(RightPadding), 40);

    public static readonly StyledProperty<double> CursorPositionProperty =
        AvaloniaProperty.Register<CursorCanvas, double>(
            nameof(CursorPosition), 0);

    public static readonly StyledProperty<Color> CursorColorProperty =
        AvaloniaProperty.Register<CursorCanvas, Color>(
            nameof(CursorColor), Colors.Red);

    public static readonly StyledProperty<double> CursorWidthProperty =
        AvaloniaProperty.Register<CursorCanvas, double>(
            nameof(CursorWidth), 1);

    public static readonly StyledProperty<bool> ShowCursorProperty =
        AvaloniaProperty.Register<CursorCanvas, bool>(
            nameof(ShowCursor), true);

    public static readonly StyledProperty<DateTime?> CursorTimeProperty =
        AvaloniaProperty.Register<CursorCanvas, DateTime?>(
            nameof(CursorTime));

    private ObservableCollection<TimeMarker>? _oldTimeMarkers;
    private ObservableCollection<TimeRangeMarker>? _oldTimeRangeMarkers;

    public ObservableCollection<TimeMarker>? TimeMarkers
    {
        get => GetValue(TimeMarkersProperty);
        set => SetValue(TimeMarkersProperty, value);
    }

    public ObservableCollection<TimeRangeMarker>? TimeRangeMarkers
    {
        get => GetValue(TimeRangeMarkersProperty);
        set => SetValue(TimeRangeMarkersProperty, value);
    }

    public IChartGlobal? ChartGlobal
    {
        get => GetValue(ChartGlobalProperty);
        set => SetValue(ChartGlobalProperty, value);
    }

    public double LeftPadding
    {
        get => GetValue(LeftPaddingProperty);
        set => SetValue(LeftPaddingProperty, value);
    }

    public double RightPadding
    {
        get => GetValue(RightPaddingProperty);
        set => SetValue(RightPaddingProperty, value);
    }

    public double CursorPosition
    {
        get => GetValue(CursorPositionProperty);
        set => SetValue(CursorPositionProperty, value);
    }

    public Color CursorColor
    {
        get => GetValue(CursorColorProperty);
        set => SetValue(CursorColorProperty, value);
    }

    public double CursorWidth
    {
        get => GetValue(CursorWidthProperty);
        set => SetValue(CursorWidthProperty, value);
    }

    public bool ShowCursor
    {
        get => GetValue(ShowCursorProperty);
        set => SetValue(ShowCursorProperty, value);
    }

    public DateTime? CursorTime
    {
        get => GetValue(CursorTimeProperty);
        set => SetValue(CursorTimeProperty, value);
    }

    public CursorCanvas()
    {
        // 监听 TimeMarkers 属性变化
        this.GetObservable(TimeMarkersProperty).Subscribe(newCollection =>
        {
            if (_oldTimeMarkers != null)
            {
                _oldTimeMarkers.CollectionChanged -= OnTimeMarkersCollectionChanged;
            }

            _oldTimeMarkers = newCollection;
            if (newCollection != null)
            {
                newCollection.CollectionChanged += OnTimeMarkersCollectionChanged;
            }

            InvalidateVisual();
        });

        // 监听 TimeRangeMarkers 属性变化
        this.GetObservable(TimeRangeMarkersProperty).Subscribe(newCollection =>
        {
            if (_oldTimeRangeMarkers != null)
            {
                _oldTimeRangeMarkers.CollectionChanged -= OnTimeRangeMarkersCollectionChanged;
            }

            _oldTimeRangeMarkers = newCollection;
            if (newCollection != null)
            {
                newCollection.CollectionChanged += OnTimeRangeMarkersCollectionChanged;
            }

            InvalidateVisual();
        });

        // 监听其他属性变化
        this.GetObservable(ChartGlobalProperty).Subscribe(_ => InvalidateVisual());
        this.GetObservable(LeftPaddingProperty).Subscribe(_ => InvalidateVisual());
        this.GetObservable(RightPaddingProperty).Subscribe(_ => InvalidateVisual());
        this.GetObservable(CursorPositionProperty).Subscribe(_ => InvalidateVisual());
        this.GetObservable(CursorColorProperty).Subscribe(_ => InvalidateVisual());
        this.GetObservable(CursorWidthProperty).Subscribe(_ => InvalidateVisual());
        this.GetObservable(ShowCursorProperty).Subscribe(_ => InvalidateVisual());
        this.GetObservable(CursorTimeProperty).Subscribe(_ => InvalidateVisual());
    }

    private void OnTimeMarkersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        InvalidateVisual();
    }

    private void OnTimeRangeMarkersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        // 1. 绘制 TimeRangeMarker（底层）
        DrawTimeRangeMarkers(context, bounds);

        // 2. 绘制 TimeMarker
        DrawTimeMarkers(context, bounds);

        // 3. 绘制 Cursor（顶层）
        DrawCursor(context, bounds);
    }

    /// <summary>
    /// 格式化相对时间
    /// </summary>
    private static string FormatRelativeTime(TimeSpan offset)
    {
        if (offset.TotalSeconds < 0)
        {
            return $"-{FormatPositiveTime(offset.Duration())}";
        }
        return FormatPositiveTime(offset);
    }

    /// <summary>
    /// 格式化正数时间差
    /// </summary>
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
            return $"{duration.TotalSeconds:F2}s";
        }
        else
        {
            return $"{duration.TotalMilliseconds:F0}ms";
        }
    }

    private void DrawTimeRangeMarkers(DrawingContext context, Rect bounds)
    {
        if (TimeRangeMarkers == null || ChartGlobal == null) return;

        foreach (var marker in TimeRangeMarkers)
        {
            if (!marker.IsVisible) continue;

            var startX = ChartGlobal.TimeToX(marker.StartTime);
            var endX = ChartGlobal.TimeToX(marker.EndTime);

            if (double.IsNaN(startX) || double.IsNaN(endX)) continue;
            if (endX < LeftPadding || startX > bounds.Width - RightPadding) continue;

            startX = Math.Max(startX, LeftPadding);
            endX = Math.Min(endX, bounds.Width - RightPadding);

            var rect = new Rect(startX, 0, endX - startX, bounds.Height);
            var brush = new SolidColorBrush(marker.Color);
            context.FillRectangle(brush, rect);

            if (!string.IsNullOrEmpty(marker.Label))
            {
                var text = new FormattedText(
                    marker.Label,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    12,
                    new SolidColorBrush(Color.FromRgb(224, 224, 224)));

                context.DrawText(text, new Point(startX + 4, 4));
            }
        }
    }

    private void DrawTimeMarkers(DrawingContext context, Rect bounds)
    {
        if (TimeMarkers == null || ChartGlobal == null) return;

        foreach (var marker in TimeMarkers)
        {
            if (!marker.IsVisible) continue;

            var x = ChartGlobal.TimeToX(marker.Time);
            if (double.IsNaN(x)) continue;
            if (x < LeftPadding || x > bounds.Width - RightPadding) continue;

            var pen = new Pen(new SolidColorBrush(marker.Color), 1, new DashStyle(new double[] { 4, 2 }, 0));
            context.DrawLine(pen, new Point(x, 0), new Point(x, bounds.Height));

            if (!string.IsNullOrEmpty(marker.Label))
            {
                // 相对时间模式下，在标签后添加相对时间
                string displayLabel = marker.Label;
                if (ChartGlobal.UseRelativeTime)
                {
                    var offset = marker.Time - ChartGlobal.RelativeTimeBase;
                    displayLabel = $"{marker.Label} ({FormatRelativeTime(offset)})";
                }

                var text = new FormattedText(
                    displayLabel,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    12,
                    new SolidColorBrush(marker.Color));

                context.DrawText(text, new Point(x + 4, 4));
            }
        }
    }

    private void DrawCursor(DrawingContext context, Rect bounds)
    {
        if (!ShowCursor) return;
        if (CursorPosition < LeftPadding || CursorPosition > bounds.Width - RightPadding) return;

        // 绘制光标线
        var pen = new Pen(new SolidColorBrush(CursorColor), CursorWidth);
        context.DrawLine(pen, new Point(CursorPosition, 0), new Point(CursorPosition, bounds.Height));

        // 绘制时间文本
        if (CursorTime.HasValue && ChartGlobal != null)
        {
            string timeText;
            if (ChartGlobal.UseRelativeTime)
            {
                // 相对时间模式
                var offset = CursorTime.Value - ChartGlobal.RelativeTimeBase;
                timeText = FormatRelativeTime(offset);
            }
            else
            {
                timeText = CursorTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            
            var text = new FormattedText(
                timeText,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                12,
                new SolidColorBrush(Color.FromRgb(125, 211, 252)));

            // 动态调整文本位置：右侧时显示在左边，左侧时显示在右边
            var textWidth = text.Width;
            double textX;
            
            if (CursorPosition + textWidth + 8 > bounds.Width - RightPadding)
            {
                // 光标在右侧，文本显示在光标左边
                textX = CursorPosition - textWidth - 4;
            }
            else
            {
                // 光标在左侧或中间，文本显示在光标右边
                textX = CursorPosition + 4;
            }

            var textY = bounds.Bottom-12;
            context.DrawText(text, new Point(textX, textY));
        }
    }
}
