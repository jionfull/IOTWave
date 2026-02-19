using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using IOTChartBuddy.Controls;
using IotWave.Models;

namespace IotWave.Views;

public partial class StatusPanel : ChartPanelBase
{
   

    // 属性
    public static readonly StyledProperty<StatuSeriesGroup> ItemsProperty =
        AvaloniaProperty.Register<StatusPanel, StatuSeriesGroup>(nameof(Items), new StatuSeriesGroup());

    public static readonly StyledProperty<IBrush> BorderBrushProperty =
        AvaloniaProperty.Register<StatusPanel, IBrush>(nameof(BorderBrush), Brushes.Black);

    public static readonly StyledProperty<double> BorderThicknessProperty =
        AvaloniaProperty.Register<StatusPanel, double>(nameof(BorderThickness), 1.0);

    public StatuSeriesGroup Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public IBrush BorderBrush
    {
        get => GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    public double BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    // 当前状态值属性
    public static readonly StyledProperty<string> CurrentStatusProperty =
        AvaloniaProperty.Register<StatusPanel, string>(nameof(CurrentStatus), "");

    public string CurrentStatus
    {
        get => GetValue(CurrentStatusProperty);
        set => SetValue(CurrentStatusProperty, value);
    }

    public StatusPanel()
    {
        InitializeComponent();
        this.GetObservable(ItemsProperty).Subscribe(_ => InvalidateVisual());
        this.GetObservable(ChartGlobalProperty).Subscribe(_=>InitBorder());
        
        // 监听父控件的CursorTime变化
        this.AttachedToVisualTree += (s, e) =>
        {
          
        };
    }

    private void InitBorder()
    {
        if(ChartGlobal == null) return;
        this.MainBorder.Margin = new Thickness(ChartGlobal.LeftPadding,0,ChartGlobal.RightPadding,0);
        if (ChartGlobal is WaveListPanel waveListPanel)
        {
            waveListPanel.GetObservable(WaveListPanel.CursorTimeProperty)
                .Subscribe(_ =>
                {
                    UpdateCurrentStatus();
                });
        }
    }

    public override void Render(DrawingContext context)
    {
        if (Bounds.Width <= 0 || Bounds.Height <= 0) return;

        DrawGrid(context);
        DrawStatus(context);
    }

    private void DrawGrid(DrawingContext context)
    {
        var leftPadding = ChartGlobal?.LeftPadding ?? 0;
        var rightPadding = ChartGlobal?.RightPadding ?? 0;

        // 绘制背景（整个控件区域）
        var background = PanelBackground ?? Brushes.White;
        context.FillRectangle(background, Bounds);

        // 绘制边框（与 MainBorder 的 Margin 一致，只在内容区域绘制）
        var borderRect = new Rect(leftPadding, 0, Bounds.Width - leftPadding - rightPadding, Bounds.Height);
        var borderPen = new Pen(BorderBrush, BorderThickness);
        context.DrawRectangle(null, borderPen, borderRect);

        // 绘制网格线
        var gridBrush = ChartGlobal?.GridBrush ?? Brushes.LightGray;
        var gridPen = new Pen(gridBrush, ChartGlobal?.GridThickness ?? 1);

        // 左右边界线
        context.DrawLine(gridPen, new Point(ChartGlobal?.LeftPadding ?? 0, 0),
            new Point(ChartGlobal?.LeftPadding ?? 0, Bounds.Height));
        context.DrawLine(gridPen, new Point(Bounds.Width - (ChartGlobal?.RightPadding ?? 0), 0),
            new Point(Bounds.Width - (ChartGlobal?.RightPadding ?? 0), Bounds.Height));

        // 垂直时间网格线
        var drawAreaWidth = Bounds.Width - leftPadding - rightPadding;

        if (ChartGlobal == null || drawAreaWidth <= 0) return;

        var beginTime = ChartGlobal.GetVisibleStartTime();
        var endTime = ChartGlobal.GetVisibleEndTime();
        var tickIntervals = ChartGlobal.CalculateTickIntervals();
        var pixelsPerTick = ChartGlobal.GetPixelsPerTick();

        foreach (var tickTime in tickIntervals)
        {
            var x = (tickTime - beginTime.Ticks) * pixelsPerTick + leftPadding;

            if (x < leftPadding || x > Bounds.Width - rightPadding)
                continue;

            context.DrawLine(gridPen, new Point(x, 0), new Point(x, Bounds.Height));
        }

      

        // 绘制光标当前的状态值（左侧）
        //DrawCursorStatusValue(context);
    }

    private void DrawStatus(DrawingContext context)
    {
        if (Items?.Points == null || Items.Points.Count == 0) return;
        if (ChartGlobal == null) return;

        var leftPadding = ChartGlobal.LeftPadding;
        var rightPadding = ChartGlobal.RightPadding;
        var drawArea = new Rect(leftPadding, 0, Bounds.Width - leftPadding - rightPadding, Bounds.Height);

        var visibleTimeStart = ChartGlobal.StartTime;
        var visibleTimeEnd = ChartGlobal.EndTime;

        // 添加边界缓冲
        var bufferWidth = (visibleTimeEnd - visibleTimeStart).TotalSeconds / drawArea.Width;
        var bufferTime = TimeSpan.FromSeconds(bufferWidth);
        var startTime = visibleTimeStart.Subtract(bufferTime);
        var endTime = visibleTimeEnd.Add(bufferTime);

        // 预过滤：获取可见范围内的点
        var sortedPoints = Items.Points.OrderBy(p => p.Time).ToList();
        var startIndex = sortedPoints.FindIndex(p => p.Time >= startTime);
        var endIndex = sortedPoints.FindIndex(p => p.Time > endTime);

        // 边界检查
        if (startIndex < 0) startIndex = 0;
        else if (startIndex > 0)
        {
            // 确保 startIndex 包含可见区域之前的最后一个点，以保持状态连续性
            startIndex--;
        }

        if (endIndex < 0) endIndex = sortedPoints.Count;

        // 计算线宽：根据状态值的数量，使每条线占据整个面板高度的一部分
        var statusValues = Items?.Brushes?.Keys.ToList() ?? new List<int>();
        var statusCount = statusValues.Count > 0 ? statusValues.Count : 1;
        var lineThickness = Bounds.Height ;

        // 绘制每条水平线段
        for (int i = startIndex; i < endIndex; i++)
        {
            var point = sortedPoints[i];
            var x = ChartGlobal.TimeToX(point.Time);
            var brush = GetBrushForValue(point.Value);

            // 计算线段的结束位置（下一个点或可见区域边界）
            double endX;
            if (i + 1 < sortedPoints.Count)
            {
                endX = ChartGlobal.TimeToX(sortedPoints[i + 1].Time);
            }
            else
            {
                endX = ChartGlobal.TimeToX(endTime);
            }

            // 只绘制在可见区域内的部分
            var lineStartX = Math.Max(drawArea.Left, x);
            var lineEndX = Math.Min(drawArea.Right, endX);

            if (lineEndX > lineStartX)
            {
                // 使用矩形绘制以支持图像 Brush
                var rect = new Rect(lineStartX, 0, lineEndX - lineStartX, Bounds.Height);
                context.FillRectangle(brush, rect);
            }
        }
    }

    /// <summary>
    /// 获取并更新当前状态值
    /// </summary>
    private void UpdateCurrentStatus()
    {
        if (ChartGlobal == null)
        {
            CurrentStatus = "";
            return;
        }

        var cursorTime = ChartGlobal.CursorTime;

        // 查找光标时间点的状态值
        if (Items?.Points == null || Items.Points.Count == 0)
        {
            CurrentStatus = "";
            return;
        }

        var sortedPoints = Items.Points.OrderBy(p => p.Time).ToList();
        int cursorValue = sortedPoints[0].Value;

        // 找到光标时间点之前最后一个状态点
        foreach (var point in sortedPoints)
        {
            if (point.Time <= cursorTime)
            {
                cursorValue = point.Value;
            }
            else
            {
                break;
            }
        }

        // 获取状态描述
        var statusText = Items?.Descriptions?.ContainsKey(cursorValue) == true
            ? Items.Descriptions[cursorValue]
            : cursorValue.ToString();

        CurrentStatus = statusText;
    }

    /// <summary>
    /// 绘制光标当前的状态值（左侧显示）
    /// </summary>
    private void DrawCursorStatusValue(DrawingContext context)
    {
        // 更新当前状态值
        UpdateCurrentStatus();

        // 尝试从父控件获取 CursorTime
        if (this.Parent is not WaveListPanel waveListPanel || !waveListPanel.CursorTime.HasValue)
            return;

        var cursorTime = waveListPanel.CursorTime.Value;

        // 查找光标时间点的状态值
        if (Items?.Points == null || Items.Points.Count == 0) return;

        var sortedPoints = Items.Points.OrderBy(p => p.Time).ToList();
        int cursorValue = sortedPoints[0].Value;

        // 找到光标时间点之前最后一个状态点
        foreach (var point in sortedPoints)
        {
            if (point.Time <= cursorTime)
            {
                cursorValue = point.Value;
            }
            else
            {
                break;
            }
        }

        // 获取状态描述
        var statusText = Items?.Descriptions?.ContainsKey(cursorValue) == true
            ? Items.Descriptions[cursorValue]
            : cursorValue.ToString();

        // 在左侧垂直居中显示状态值
        var labelText = CreateFormattedText(statusText);
        var labelX = 5;
        var labelY = (Bounds.Height - labelText.Height) / 2;
        context.DrawText(labelText, new Point(labelX, labelY));
    }

    private IBrush GetBrushForValue(int value)
    {
        return Items?.Brushes?.ContainsKey(value) == true
            ? Items.Brushes[value]
            : Brushes.Black;
    }

    private double GetYPosition(int value)
    {
        // 始终在水平方向上绘制（垂直居中）
        return Bounds.Height / 2;
    }
}
