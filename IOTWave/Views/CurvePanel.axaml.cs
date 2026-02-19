using System.Linq;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Media;
using IOTChartBuddy.Controls;
using IOTWave.Models;
using ReactiveUI;

namespace IOTWave.Views;

public partial class CurvePanel : ChartPanelBase
{
   
        private double _yMin = 0;
        private double _yMax = 100;
        private double _yOffset = 0;
        private double _yScale = 1.0;
        private bool _isDragging = false;
        private Point _lastDragPoint;
        private readonly YAxisRenderer _yAxisRenderer;

        // 属性
        public static readonly StyledProperty<double> DesiredHeightProperty =
            AvaloniaProperty.Register<CurvePanel, double>(nameof(DesiredHeight), 200.0);

        public static readonly StyledProperty<IList<YMarker>> YMarkersProperty =
            AvaloniaProperty.Register<CurvePanel, IList<YMarker>>(nameof(YMarkers));

        public static readonly StyledProperty<bool> ShowYAxisProperty =
            AvaloniaProperty.Register<CurvePanel, bool>(nameof(ShowYAxis), true);

        public static readonly StyledProperty<CurveGroup> ItemsProperty =
            AvaloniaProperty.Register<CurvePanel, CurveGroup>(nameof(Items), new CurveGroup());

        public static readonly StyledProperty<bool> ShowCurrentValueProperty =
            AvaloniaProperty.Register<CurvePanel, bool>(nameof(ShowCurrentValue), false);


        public double DesiredHeight
        {
            get => GetValue(DesiredHeightProperty);
            set => SetValue(DesiredHeightProperty, value);
        }

        public IList<YMarker> YMarkers
        {
            get => GetValue(YMarkersProperty) ?? new List<YMarker>();
            set
            {
                SetValue(YMarkersProperty, value);
               
            }
        }

        public bool ShowYAxis
        {
            get => GetValue(ShowYAxisProperty);
            set => SetValue(ShowYAxisProperty, value);
        }

        /// <summary>
        /// 是否绘制底部分割线，最后一个 CurvePanel 应设置为 false 以避免与 TimeAxis 重叠
        /// </summary>
        public bool DrawBottomSeparator
        {
            get => _yAxisRenderer.DrawBottomSeparator;
            set => _yAxisRenderer.DrawBottomSeparator = value;
        }

        public CurveGroup Items
        {
            get => GetValue(ItemsProperty);
            set
            {
                SetValue(ItemsProperty, value);
                SetupCurveDataListeners();
            }
        }

        /// <summary>
        /// 是否在图例中显示光标位置的当前值
        /// </summary>
        public bool ShowCurrentValue
        {
            get => GetValue(ShowCurrentValueProperty);
            set => SetValue(ShowCurrentValueProperty, value);
        }


        public CurvePanel()
        {
            InitializeComponent();
            YMarkers = new List<YMarker>();
            _yAxisRenderer = new YAxisRenderer(this, ChartGlobal, CreateFormattedText);

            // 鼠标事件处理
            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
            PointerWheelChanged += OnPointerWheelChanged;

            this.GetObservable(ItemsProperty).Subscribe(_ => SetupCurveDataListeners());
            this.GetObservable(ChartGlobalProperty).Subscribe(OnChartGlobalChanged);
            this.GetObservable(YMarkersProperty).Subscribe(_ => InvalidateVisual());
        }

        private void OnShowCurrentValueChanged(bool showCurrentValue)
        {
            if (curveCheck == null) return;
            
            // 更新 ListBox 的 Classes
            if (showCurrentValue)
            {
                curveCheck.Classes.Remove("curveLegend");
                curveCheck.Classes.Add("curveLegendWithValue");
            }
            else
            {
                curveCheck.Classes.Remove("curveLegendWithValue");
                curveCheck.Classes.Add("curveLegend");
            }
        }

        private void OnChartGlobalChanged(IChartGlobal obj)
        {
            if (obj == null)
            {
                return;
            }


             this.curveCheck.Padding = new Thickness(obj.LeftPadding, 0, obj.RightPadding, 0);

            // 订阅 ResetYViewRequested 事件
            obj.ResetYViewRequestedEvent += ResetView;

            // 订阅光标时间变化 - 通过 WaveListPanel 的属性订阅
            if (obj is WaveListPanel waveListPanel)
            {
                waveListPanel.GetObservable(WaveListPanel.CursorTimeProperty).Subscribe(OnCursorTimeChanged);

                // 订阅 ShowCurrentValue 变化
                waveListPanel.GetObservable(WaveListPanel.ShowCurrentValueProperty).Subscribe(OnShowCurrentValueChanged);
            }
        }

        private void OnCursorTimeChanged(DateTime? cursorTime)
        {
            if (Items?.Curves == null) return;
            
            // 更新所有曲线的当前值
            foreach (var curve in Items.Curves)
            {
                curve.UpdateCurrentValue(cursorTime);
            }
        }

        private void SetupCurveDataListeners()
        {
            if (Items == null || Items.Curves == null) return;

            foreach (var curve in Items.Curves)
            {
                curve.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(CurveData.IsVisible))
                    {
                        InvalidateVisual();
                    }
                };
            }

            // 数据绑定成功后，自动调用 ResetView
            ResetView();
        }


        private void AutoScaleY()
        {
            if (Items == null || !Items.Curves.Any()) return;

            var allPoints = Items.Curves.SelectMany(c => c.Points).ToList();
            if (!allPoints.Any()) return;

            _yMin = allPoints.Min(p => p.Value);
            _yMax = allPoints.Max(p => p.Value);

            // 添加一些边距
            double margin = (_yMax - _yMin) * 0.1;
            if (margin == 0) margin = 1;

            _yMin -= margin;
            _yMax += margin;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                // 只有在按住 Control 键时才进行 Y 轴操作，否则让 ChartContainer 处理 X 轴操作
                if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    _isDragging = true;
                    _lastDragPoint = e.GetPosition(this);
                    e.Handled = true; // Y轴操作，拦截事件
                }
                else
                {
                    e.Handled = false; // 让 ChartContainer 处理 X 轴平移
                }
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPoint = e.GetPosition(this);
                double deltaY = currentPoint.Y - _lastDragPoint.Y;

                // Y轴平移
                _yOffset += deltaY * (_yMax - _yMin) / Bounds.Height / _yScale;

                _lastDragPoint = currentPoint;
                InvalidateVisual();
                e.Handled = true;
            }
            else
            {
                e.Handled = false; // 让 ChartContainer 处理 X 轴平移
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isDragging = false;
        }

        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            // 按住 Shift 键时进行 Y 轴缩放，否则让 ChartContainer 处理 X 轴缩放
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                // Y轴缩放
                double scaleFactor = e.Delta.Y > 0 ? 0.9 : 1.1;
                _yScale *= scaleFactor;
                _yScale = Math.Max(0.1, Math.Min(10, _yScale)); // 限制缩放范围
                InvalidateVisual();
                e.Handled = true; // 拦截事件
            }
            else
            {
                e.Handled = false; // 让 ChartContainer 处理 X 轴缩放
            }
        }

        public override void Render(DrawingContext context)
        {
            if (Bounds.Width <= 0 || Bounds.Height <= 0) return;

            UpdateYAxisRenderer();
            DrawGrid(context);
            _yAxisRenderer.DrawYAxis(context);
        
            DrawCurves(context);   
            _yAxisRenderer.DrawYMarkers(context);
        }

        private void DrawCurves(DrawingContext context)
        {
            var drawArea = new Rect(
                ChartGlobal.LeftPadding,
                0,
                Bounds.Width - ChartGlobal.LeftPadding - ChartGlobal.RightPadding,
                Bounds.Height
            );

            if (Items == null || Items.Curves == null) return;

            // 只绘制可见的曲线
            foreach (var curve in Items.Curves.Where(c => c.IsVisible))
            {
                DrawSingleCurve(context, curve, drawArea);
            }
        }

        /// <summary>
        /// 计算线段与垂直边界的交点
        /// </summary>
        private Point GetIntersection(Point p1, Point p2, double xBoundary)
        {
            // 线性插值计算交点Y坐标
            var t = (xBoundary - p1.X) / (p2.X - p1.X);
            var y = p1.Y + t * (p2.Y - p1.Y);
            return new Point(xBoundary, y);
        }

        // 缓存画笔以避免重复创建
        private readonly Dictionary<Color, Pen> _penCache = new Dictionary<Color, Pen>();
        private readonly Dictionary<Color, SolidColorBrush> _brushCache = new Dictionary<Color, SolidColorBrush>();

        private Pen GetCachedPen(Color color, double thickness)
        {
            var key = new Color(color.A, color.R, color.G, color.B);
            if (!_penCache.TryGetValue(key, out var pen))
            {
                pen = new Pen(new SolidColorBrush(color), thickness);
                _penCache[key] = pen;
            }
            return pen;
        }

        private SolidColorBrush GetCachedBrush(Color color)
        {
            var key = new Color(color.A, color.R, color.G, color.B);
            if (!_brushCache.TryGetValue(key, out var brush))
            {
                brush = new SolidColorBrush(color);
                _brushCache[key] = brush;
            }
            return brush;
        }

        /// <summary>
        /// 保留极值的下采样：在每个像素区间内保留极值（最大值和最小值），确保不丢失峰值
        /// </summary>
        private List<(Point point, int originalIndex)> DownsamplePreservingExtremes(
            List<TimePoint> points, int startIndex, int endIndex, double drawAreaWidth, Rect drawArea)
        {
            var result = new List<(Point, int)>();

            // 如果点数不多，无需下采样
            var visiblePointCount = endIndex - startIndex;
            if (visiblePointCount <= drawAreaWidth)
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    var p = points[i];
                    result.Add((new Point(ChartGlobal.TimeToX(p.Time), ValueToY(p.Value)), i));
                }
                return result;
            }

            // 每个像素代表的 ticks 数
            var ticksPerPx = (ChartGlobal.EndTime.Ticks - ChartGlobal.StartTime.Ticks) / drawAreaWidth;
            var pointsPerPixel = (int)(visiblePointCount / drawAreaWidth);

            // 按像素区间分组，每个区间保留：起点、终点、最大值、最小值
            for (int i = startIndex; i < endIndex;)
            {
                // 当前区间范围 [i, endOfBucket)
                int endOfBucket = Math.Min(i + pointsPerPixel, endIndex);

                if (endOfBucket - i <= 2)
                {
                    // 点太少，全部保留
                    for (int j = i; j < endOfBucket; j++)
                    {
                        var p = points[j];
                        result.Add((new Point(ChartGlobal.TimeToX(p.Time), ValueToY(p.Value)), j));
                    }
                }
                else
                {
                    // 保留起点
                    var startPoint = points[i];
                    result.Add((new Point(ChartGlobal.TimeToX(startPoint.Time), ValueToY(startPoint.Value)), i));

                    // 在区间内找极值
                    int maxIndex = i + 1;
                    int minIndex = i + 1;
                    double maxY = ValueToY(points[i + 1].Value);
                    double minY = maxY;

                    for (int j = i + 1; j < endOfBucket; j++)
                    {
                        var y = ValueToY(points[j].Value);
                        if (y > maxY)
                        {
                            maxY = y;
                            maxIndex = j;
                        }
                        if (y < minY)
                        {
                            minY = y;
                            minIndex = j;
                        }
                    }

                    // 添加极值点（避免重复）
                    if (maxIndex != i && maxIndex != endOfBucket - 1)
                        result.Add((new Point(ChartGlobal.TimeToX(points[maxIndex].Time), maxY), maxIndex));
                    if (minIndex != i && minIndex != endOfBucket - 1 && minIndex != maxIndex)
                        result.Add((new Point(ChartGlobal.TimeToX(points[minIndex].Time), minY), minIndex));

                    // 保留终点
                    var endPoint = points[endOfBucket - 1];
                    result.Add((new Point(ChartGlobal.TimeToX(endPoint.Time), ValueToY(endPoint.Value)), endOfBucket - 1));
                }

                i = endOfBucket;
            }

            return result;
        }

        private void DrawSingleCurve(DrawingContext context, CurveData curve, Rect drawArea)
        {
            if (!curve.Points.Any())
                return;

            var yMin = _yMin;
            var yMax = _yMax;
            var yRange = yMax - yMin;

            if (yRange <= 0)
                return;

            // 计算时间范围过滤
            var visibleTimeStart = ChartGlobal.StartTime;
            var visibleTimeEnd = ChartGlobal.EndTime;

            // 添加边界缓冲，确保边界处的线段正确绘制
            var bufferWidth = (visibleTimeEnd - visibleTimeStart).TotalSeconds / drawArea.Width;
            var bufferTime = TimeSpan.FromSeconds(bufferWidth);

            // 预过滤：只获取时间范围内可能显示点（二分查找优化）
            var startTime = visibleTimeStart.Subtract(bufferTime);
            var endTime = visibleTimeEnd.Add(bufferTime);

            // 使用二分查找快速定位索引范围
            var startIndex = curve.Points.BinarySearch(new TimePoint { Time = startTime, Value = 0 }, TimePoint.TimeComparer.Instance);
            var endIndex = curve.Points.BinarySearch(new TimePoint { Time = endTime, Value = 0 }, TimePoint.TimeComparer.Instance);

            // 处理二分查找的返回值（可能返回负数表示未找到）
            if (startIndex < 0) startIndex = ~startIndex;
            if (endIndex < 0) endIndex = ~endIndex;

            // 确保索引有效
            startIndex = Math.Max(0, startIndex);
            endIndex = Math.Min(curve.Points.Count, endIndex);

            // 如果没有可见点，直接返回
            if (startIndex >= endIndex)
                return;

            // 使用保留极值的下采样
            var downsampledPoints = DownsamplePreservingExtremes(curve.Points, startIndex, endIndex, drawArea.Width, drawArea);

            var curvePen = GetCachedPen(curve.Color, curve.LineWidth);
            var geometry = new StreamGeometry();

            using (var ctx = geometry.Open())
            {
                if (downsampledPoints.Count > 0)
                {
                    Point? firstPoint = null;
                    Point prevTransformed = downsampledPoints[0].point;

                    for (int i = 0; i < downsampledPoints.Count; i++)
                    {
                        var current = downsampledPoints[i].point;

                        bool prevVisible = prevTransformed.X >= drawArea.Left && prevTransformed.X <= drawArea.Right;
                        bool currentVisible = current.X >= drawArea.Left && current.X <= drawArea.Right;

                        if (prevVisible && currentVisible)
                        {
                            // 两点都在可见区域内
                            if (!firstPoint.HasValue)
                            {
                                firstPoint = prevTransformed;
                                ctx.BeginFigure(prevTransformed, false);
                            }

                            ctx.LineTo(current);
                        }
                        else if (prevVisible && !currentVisible)
                        {
                            // 从区域内到区域外：计算交点
                            if (current.X > drawArea.Right)
                            {
                                var intersection = GetIntersection(prevTransformed, current, drawArea.Right);
                                if (!firstPoint.HasValue)
                                {
                                    firstPoint = prevTransformed;
                                    ctx.BeginFigure(prevTransformed, false);
                                }
                                ctx.LineTo(intersection);
                            }
                        }
                        else if (!prevVisible && currentVisible)
                        {
                            // 从区域外到区域内：计算交点并开始绘制
                            if (prevTransformed.X < drawArea.Left)
                            {
                                var intersection = GetIntersection(prevTransformed, current, drawArea.Left);
                                ctx.BeginFigure(intersection, false);
                                ctx.LineTo(current);
                                firstPoint = intersection;
                            }
                        }
                        else if (!prevVisible && !currentVisible)
                        {
                            // 两点都在区域外，但可能跨越整个区域
                            if (prevTransformed.X < drawArea.Left && current.X > drawArea.Right)
                            {
                                var startIntersection = GetIntersection(prevTransformed, current, drawArea.Left);
                                var endIntersection = GetIntersection(prevTransformed, current, drawArea.Right);
                                ctx.BeginFigure(startIntersection, false);
                                ctx.LineTo(endIntersection);
                            }
                        }

                        prevTransformed = current;
                    }
                }
            }

            context.DrawGeometry(null, curvePen, geometry);

            // 绘制数据点（如果启用）- 使用下采样后的点
            var tickPrePixel = (ChartGlobal.EndTime.Ticks - ChartGlobal.StartTime.Ticks) / drawArea.Width;
            if (curve.ShowPoints && tickPrePixel < curve.PointShowLimit * TimeSpan.TicksPerSecond)
            {
                var pointBrush = GetCachedBrush(curve.Color);
                const double pointSize = 3;

                // 绘制下采样后的关键点
                foreach (var (point, _) in downsampledPoints)
                {
                    // 只绘制在可见区域内的点
                    if (point.X >= drawArea.Left && point.X <= drawArea.Right &&
                        point.Y >= drawArea.Top && point.Y <= drawArea.Bottom)
                    {
                        var ellipse = new EllipseGeometry(
                            new Rect(point.X - pointSize, point.Y - pointSize, pointSize * 2, pointSize * 2)
                        );
                        context.DrawGeometry(pointBrush, null, ellipse);
                    }
                }
            }
        }

        protected virtual void DrawGrid(DrawingContext context)
        {
            if (Bounds.Width <= 0 || Bounds.Height <= 0) return;
            if (ChartGlobal == null) return;

            // 绘制背景
            var background = PanelBackground ?? Brushes.White;
            context.FillRectangle(background, Bounds);

            // 绘制网格线，使用 TimeXConverter 中的 GridBrush 和 GridThickness
            var gridBrush = ChartGlobal.GridBrush ?? Brushes.LightGray;
            var gridPen = new Pen(gridBrush, ChartGlobal.GridThickness);

            // 垂直网格线（时间网格）
            DateTime startTime = GetVisibleStartTime();
            DateTime endTime = GetVisibleEndTime();
            double timeRange = (endTime - startTime).TotalSeconds;

            // 根据时间范围决定网格间隔

            var pixelsPerTick = ChartGlobal.GetPixelsPerTick();
            var axisY = 1;
            //DrawLeft Ticks;
            context.DrawLine(gridPen, new Point(ChartGlobal.LeftPadding, 0),
                new Point(ChartGlobal.LeftPadding, Bounds.Height));
            context.DrawLine(gridPen, new Point(Bounds.Width - ChartGlobal.RightPadding, 0),
                new Point(Bounds.Width - ChartGlobal.RightPadding, Bounds.Height));
            var beginTime = ChartGlobal.GetVisibleStartTime();

            var tickIntervals = ChartGlobal.CalculateTickIntervals();
            var _baseInterval = ChartGlobal.BaseInterval;

            foreach (var tickTime in tickIntervals)
            {
                if (tickTime - beginTime.Ticks < _baseInterval * 0.5)
                {
                    continue;
                }

                if (endTime.Ticks - tickTime < pixelsPerTick * 0.5)
                {
                    continue;
                }

                // 计算像素位置
                var x = (tickTime - beginTime.Ticks) * pixelsPerTick + ChartGlobal.LeftPadding;

                if (x < 0 || x > Bounds.Width)
                    continue;


                context.DrawLine(gridPen, new Point(x, 0), new Point(x, Bounds.Height));
            }
        }

        private Color GetCurveColor(CurveData curve)
        {
            // 简单的颜色分配策略
            int index = this.Items.Curves.IndexOf(curve);
            Color[] colors =
            {
                Colors.Blue, Colors.Red, Colors.Green, Colors.Orange,
                Colors.Purple, Colors.Brown, Colors.Pink, Colors.Cyan
            };
            return colors[index % colors.Length];
        }

        private void UpdateYAxisRenderer()
        {
            _yAxisRenderer.YMin = _yMin;
            _yAxisRenderer.YMax = _yMax;
            _yAxisRenderer.YOffset = _yOffset;
            _yAxisRenderer.YScale = _yScale;
            _yAxisRenderer.YMarkers = YMarkers;
            _yAxisRenderer.Bounds = Bounds;
        }

        private TimePoint? FindClosestPoint(List<TimePoint> points, DateTime targetTime)
        {
            if (points.Count == 0) return null;

            return points.OrderBy(p => Math.Abs((p.Time - targetTime).TotalSeconds)).First();
        }

        public double ValueToY(double value)
        {
            return _yAxisRenderer.ValueToY(value);
        }

        public void ResetView()
        {
            _yOffset = 0;
            _yScale = 1.0;
            AutoScaleY();
            InvalidateVisual();
        }

        /// <summary>
        /// 应用 Y 轴缩放
        /// </summary>
        public void ApplyYScale(double scaleFactor)
        {
            _yScale *= scaleFactor;
            _yScale = Math.Max(0.1, Math.Min(10, _yScale)); // 限制缩放范围
            InvalidateVisual();
        }
}
