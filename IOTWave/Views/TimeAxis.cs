using System.Diagnostics;
using Avalonia.Input;


namespace IotWave.Views;

public class TimeAxis : Control
{
    public static readonly StyledProperty<IChartGlobal> ChartGlobalProperty =
        AvaloniaProperty.Register<TimeAxis, IChartGlobal>(nameof(ChartGlobal));

    public IChartGlobal ChartGlobal
    {
        get => GetValue(ChartGlobalProperty);
        set => SetValue(ChartGlobalProperty, value);
    }


    // 刻度配置
    public AxisScaleConfig ScaleConfig { get; set; } = new AxisScaleConfig();

    // 时间格式化器
    public ITimeFormatter TimeFormatter { get; set; } = new DefaultTimeFormatter();


    // 画笔和样式 - 优化黑色背景配色
    private static readonly IPen AxisPen = new Pen(new SolidColorBrush(Color.FromRgb(0, 180, 216)), 1);
    private static readonly IBrush TextBrush = new SolidColorBrush(Color.FromRgb(125, 211, 252));
    private static readonly Typeface TextTypeface = new Typeface("Arial");

    /// <summary>
    /// 鼠标滚轮缩放事件，参数为鼠标X坐标和时间
    /// </summary>
    public event EventHandler<MouseWheelZoomEventArgs>? MouseWheelZoom;

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        var position = e.GetCurrentPoint(this).Position;
        var time = ChartGlobal.XToTime(position.X);
        MouseWheelZoom?.Invoke(this, new MouseWheelZoomEventArgs(e.Delta.Y, time));
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Bounds.Width <= 0 || Bounds.Height <= 0)
            return;

        if (ChartGlobal.EndTime <= ChartGlobal.StartTime)
        {
            return;
        }

        try
        {
            // 1. 绘制轴线
            DrawAxisLine(context);


            // 2. 计算和绘制刻度（基于缩放级别）
            var tickIntervals = ChartGlobal.CalculateTickIntervals();
            DrawTicksAndLabels(context, tickIntervals);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"TimeAxis render error: {ex.Message}");
        }
    }


    private void DrawAxisLine(DrawingContext context)
    {
        var axisY = Bounds.Height - 1;

        context.DrawLine(AxisPen, new Point(ChartGlobal.LeftPadding, 1),
            new Point(Bounds.Width - ChartGlobal.RightPadding, 1));
    }

    private long _baseInterval;


    private void DrawTicksAndLabels(DrawingContext context, List<long> tickIntervals)
    {
        var pixelsPerTick = (Bounds.Width - ChartGlobal.LeftPadding - ChartGlobal.RightPadding) /
                            (double)(ChartGlobal.EndTime.Ticks - ChartGlobal.StartTime.Ticks);
        var axisY = 1;
        //DrawLeft Ticks;
        context.DrawLine(AxisPen, new Point(ChartGlobal.LeftPadding, 1),
            new Point(ChartGlobal.LeftPadding, ScaleConfig.MajorTickLength));
        context.DrawLine(AxisPen, new Point(Bounds.Width - ChartGlobal.RightPadding, 1),
            new Point(Bounds.Width - ChartGlobal.RightPadding, ScaleConfig.MajorTickLength));
        DrawStartLabel(context);
        var preTicks = ChartGlobal.StartTime.Ticks;
        foreach (var tickTime in tickIntervals)
        {
           

           
            // 计算像素位置
            var x = (tickTime - ChartGlobal.StartTime.Ticks) * pixelsPerTick + ChartGlobal.LeftPadding;

            if (x < 0 || x > Bounds.Width)
                continue;

            // 绘制主刻度
            var tickTop = axisY - ScaleConfig.MajorTickLength;
            context.DrawLine(AxisPen, new Point(x, 1), new Point(x, ScaleConfig.MajorTickLength));

            // 绘制小刻度
            DrawMinorTicks(context, preTicks, tickTime, pixelsPerTick);

            // 绘制标签
            DrawTimeLabel(context, preTicks, tickTime, x, ScaleConfig.MajorTickLength);
            preTicks = tickTime;
        }
    }

    private void DrawStartLabel(DrawingContext context)
    {
        DateTime bTime = new DateTime(ChartGlobal.StartTime.Ticks);

        String labelText = bTime.ToString("HH:mm:ss");
        var formattedText = new FormattedText(
            labelText,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            TextTypeface,
            ScaleConfig.FontSize,
            TextBrush);

        var textY = ScaleConfig.MajorTickLength + 2;

        // 边界检查


        context.DrawText(formattedText, new Point(ChartGlobal.LeftPadding, textY));
        String labelText2 = bTime.ToString("M月d日");
        var formattedText2 = new FormattedText(
            labelText2,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            TextTypeface,
            ScaleConfig.FontSize,
            TextBrush);

        var textY2 = ScaleConfig.MajorTickLength + 2 + ScaleConfig.FontSize;

        // 边界检查


        context.DrawText(formattedText2, new Point(ChartGlobal.LeftPadding, textY2));
    }

    private void DrawTimeLabel(DrawingContext context, long preTicks, long ticks, double x, double y)
    {
        DateTime priTime = new DateTime(preTicks);
        DateTime tickTime = new DateTime(ticks);
        String labelText = tickTime.ToString("HH:mm:ss");
        if (tickTime.Date != priTime.Date)
        {
            labelText = $"{tickTime:MM/dd}日";
        }
        else if (tickTime.Hour != priTime.Hour)
        {
            labelText = $"{tickTime:HH:mm}";
        }
        else if (tickTime.Minute != priTime.Minute)
        {
            labelText = $"{tickTime:mm分ss}";
        }
        else if (tickTime.Second != priTime.Second)
        {
            labelText = $"{tickTime:ss秒ff}";
        }
        else if (tickTime.Millisecond != priTime.Millisecond)
        {
            labelText = $"{tickTime.Millisecond}ms";
        }
        else if (tickTime.Microsecond != priTime.Microsecond)
        {
            labelText = $"{tickTime.Microsecond}mS";
        }

        else if (tickTime.Nanosecond != priTime.Nanosecond)
        {
            labelText = $"{tickTime.Minute}nS";
        }


        var formattedText = new FormattedText(
            labelText,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            TextTypeface,
            ScaleConfig.FontSize,
            TextBrush);

        // 计算文本位置
        var textX = x - formattedText.Width / 2;
        var textY = y + 2;

        // 边界检查
        textX = Math.Max(2, Math.Min(textX, Bounds.Width - formattedText.Width - 2));

        context.DrawText(formattedText, new Point(textX, textY));
    }

    /// <summary>
    /// 在两个主刻度之间绘制小刻度
    /// </summary>
    private void DrawMinorTicks(DrawingContext context, long preTicks, long currentTick, double pixelsPerTick)
    {
        // 如果两个主刻度之间的间隔太小，不绘制小刻度
        var interval = currentTick - preTicks;
        if (interval < TimeSpan.TicksPerSecond)
            return; // 小于1秒时不绘制小刻度

        // 计算小刻度的数量（通常是4或5个，将主间隔分成5份）
        int minorTickCount = 4; // 在主刻度之间绘制4个小刻度，形成5等分
        var minorInterval = interval / 5;

        // 只在足够大的间隔下才显示小刻度
        if (minorInterval * pixelsPerTick < 10) // 如果小刻度间距小于10像素，不显示
            return;

        // 使用较细的画笔绘制小刻度
        var minorPen = new Pen(new SolidColorBrush(Color.FromRgb(64, 64, 64)), 0.5);

        for (int i = 1; i <= minorTickCount; i++)
        {
            var minorTickTime = preTicks + minorInterval * i;
            var x = (minorTickTime - ChartGlobal.StartTime.Ticks) * pixelsPerTick + ChartGlobal.LeftPadding;

            // 边界检查
            if (x < 0 || x > Bounds.Width)
                continue;

            // 绘制小刻度（长度较短）
            context.DrawLine(minorPen, new Point(x, 1), new Point(x, ScaleConfig.MinorTickLength));
        }
    }
}

public class MouseWheelZoomEventArgs : EventArgs
{
    public double Delta { get; }
    public DateTime Time { get; }

    public MouseWheelZoomEventArgs(double delta, DateTime time)
    {
        Delta = delta;
        Time = time;
    }
}