using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using IOTWave.Models;
using IOTWave.Views;
using ReactiveUI;

namespace IOTWave.Views;

/// <summary>
/// 光标移动步长类型
/// </summary>
public enum CursorMoveStep
{
    /// <summary>
    /// 微秒 (1μs)
    /// </summary>
    Microsecond = 0,
    
    /// <summary>
    /// 毫秒 (1ms)
    /// </summary>
    Millisecond = 1,
    
    /// <summary>
    /// 秒 (1s)
    /// </summary>
    Second = 2,
    
    /// <summary>
    /// 分钟 (1min)
    /// </summary>
    Minute = 3,
    
    /// <summary>
    /// 小时 (1h)
    /// </summary>
    Hour = 4,
    
    /// <summary>
    /// 天 (1d)
    /// </summary>
    Day = 5
}

public class WaveListPanel : SelectingItemsControl, IChartGlobal
{
    // 依赖属性定义

    public static readonly StyledProperty<DateTime> StartTimeProperty =
        AvaloniaProperty.Register<WaveListPanel, DateTime>(
            nameof(StartTime), DateTime.Now.AddHours(-1));

    public static readonly StyledProperty<DateTime> EndTimeProperty =
        AvaloniaProperty.Register<WaveListPanel, DateTime>(
            nameof(EndTime), DateTime.Now);

    public static readonly StyledProperty<ObservableCollection<TimeMarker>> TimeMarkersProperty =
        AvaloniaProperty.Register<WaveListPanel, ObservableCollection<TimeMarker>>(
            nameof(TimeMarkers), new ObservableCollection<TimeMarker>());

    public static readonly StyledProperty<ObservableCollection<TimeRangeMarker>> TimeRangeMarkersProperty =
        AvaloniaProperty.Register<WaveListPanel, ObservableCollection<TimeRangeMarker>>(
            nameof(TimeRangeMarkers), new ObservableCollection<TimeRangeMarker>());

    public static readonly StyledProperty<DateTime?> CursorTimeProperty =
        AvaloniaProperty.Register<WaveListPanel, DateTime?>(
            nameof(CursorTime));

    public static readonly StyledProperty<bool> ShowCursorProperty =
        AvaloniaProperty.Register<WaveListPanel, bool>(
            nameof(ShowCursor), true);

    public static readonly StyledProperty<bool> ShowTimeAxisProperty =
        AvaloniaProperty.Register<WaveListPanel, bool>(
            nameof(ShowTimeAxis), true);

    public static readonly StyledProperty<double> TimeAxisHeightProperty =
        AvaloniaProperty.Register<WaveListPanel, double>(
            nameof(TimeAxisHeight), 40);

    public static readonly StyledProperty<Color> CursorColorProperty =
        AvaloniaProperty.Register<WaveListPanel, Color>(
            nameof(CursorColor), Colors.Red);

    public static readonly StyledProperty<double> CursorWidthProperty =
        AvaloniaProperty.Register<WaveListPanel, double>(
            nameof(CursorWidth), 1);

    public static readonly StyledProperty<bool> IsInteractiveProperty =
        AvaloniaProperty.Register<WaveListPanel, bool>(
            nameof(IsInteractive), true);

    public static readonly StyledProperty<double> CursorPositionProperty =
        AvaloniaProperty.Register<WaveListPanel, double>(
            nameof(CursorPosition), 0);

    public static readonly StyledProperty<IBrush?> GridBrushProperty =
        AvaloniaProperty.Register<WaveListPanel, IBrush?>(
            nameof(GridBrush), new SolidColorBrush(Color.FromArgb(100, 0, 180, 216)));

    public static readonly StyledProperty<double> GridThicknessProperty =
        AvaloniaProperty.Register<WaveListPanel, double>(
            nameof(GridThickness), 1.0);

    public static readonly StyledProperty<IBrush?> LabelBrushProperty =
        AvaloniaProperty.Register<WaveListPanel, IBrush?>(
            nameof(LabelBrush), Brushes.Black);

    public static readonly StyledProperty<IBrush?> SeparatorBrushProperty =
        AvaloniaProperty.Register<WaveListPanel, IBrush?>(
            nameof(SeparatorBrush), Brushes.White);

    public static readonly StyledProperty<bool> AutoDistributePanelHeightProperty =
        AvaloniaProperty.Register<WaveListPanel, bool>(
            nameof(AutoDistributePanelHeight), false);

    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<WaveListPanel, ScrollBarVisibility>(
            nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Disabled);

    public static readonly StyledProperty<double> StatusPanelHeightProperty =
        AvaloniaProperty.Register<WaveListPanel, double>(
            nameof(StatusPanelHeight), 20);

    public static readonly StyledProperty<Thickness> StatusPanelMarginProperty =
        AvaloniaProperty.Register<WaveListPanel, Thickness>(
            nameof(StatusPanelMargin), new Thickness(0, 2));

    public static readonly StyledProperty<bool> UseRelativeTimeProperty =
        AvaloniaProperty.Register<WaveListPanel, bool>(
            nameof(UseRelativeTime), false);

    public static readonly StyledProperty<DateTime> RelativeTimeBaseProperty =
        AvaloniaProperty.Register<WaveListPanel, DateTime>(
            nameof(RelativeTimeBase), DateTime.Now);

    public static readonly StyledProperty<string> RelativeTimeBaseLabelProperty =
        AvaloniaProperty.Register<WaveListPanel, string>(
            nameof(RelativeTimeBaseLabel), "基准");

    public static readonly StyledProperty<CursorMoveStep> CursorMoveStepProperty =
        AvaloniaProperty.Register<WaveListPanel, CursorMoveStep>(
            nameof(CursorMoveStep), CursorMoveStep.Second);

    public static readonly StyledProperty<bool> EnableKeyboardCursorControlProperty =
        AvaloniaProperty.Register<WaveListPanel, bool>(
            nameof(EnableKeyboardCursorControl), true);

    // 命令属性
    public static readonly DirectProperty<WaveListPanel, ICommand> ZoomInCommandProperty =
        AvaloniaProperty.RegisterDirect<WaveListPanel, ICommand>(
            nameof(ZoomInCommand),
            o => o.ZoomInCommand);

    public static readonly DirectProperty<WaveListPanel, ICommand> ZoomOutCommandProperty =
        AvaloniaProperty.RegisterDirect<WaveListPanel, ICommand>(
            nameof(ZoomOutCommand),
            o => o.ZoomOutCommand);


    public DateTime StartTime
    {
        get => GetValue(StartTimeProperty);
        set { SetValue(StartTimeProperty, value); }
    }

    public DateTime EndTime
    {
        get => GetValue(EndTimeProperty);
        set => SetValue(EndTimeProperty, value);
    }

    public ObservableCollection<TimeMarker> TimeMarkers
    {
        get => GetValue(TimeMarkersProperty);
        set => SetValue(TimeMarkersProperty, value);
    }

    public ObservableCollection<TimeRangeMarker> TimeRangeMarkers
    {
        get => GetValue(TimeRangeMarkersProperty);
        set => SetValue(TimeRangeMarkersProperty, value);
    }

    public DateTime? CursorTime
    {
        get => GetValue(CursorTimeProperty);
        set => SetValue(CursorTimeProperty, value);
    }

    public double CursorPosition
    {
        get => GetValue(CursorPositionProperty);
        set => SetValue(CursorPositionProperty, value);
    }

    public IBrush? GridBrush
    {
        get => GetValue(GridBrushProperty);
        set => SetValue(GridBrushProperty, value);
    }

    public double GridThickness
    {
        get => GetValue(GridThicknessProperty);
        set => SetValue(GridThicknessProperty, value);
    }

    public IBrush? LabelBrush
    {
        get => GetValue(LabelBrushProperty);
        set => SetValue(LabelBrushProperty, value);
    }

    public IBrush? SeparatorBrush
    {
        get => GetValue(SeparatorBrushProperty);
        set => SetValue(SeparatorBrushProperty, value);
    }

    public bool AutoDistributePanelHeight
    {
        get => GetValue(AutoDistributePanelHeightProperty);
        set => SetValue(AutoDistributePanelHeightProperty, value);
    }

    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }

    public double StatusPanelHeight
    {
        get => GetValue(StatusPanelHeightProperty);
        set => SetValue(StatusPanelHeightProperty, value);
    }

    /// <summary>
    /// StatusPanel 的 Margin，计算高度时需要考虑
    /// </summary>
    public Thickness StatusPanelMargin
    {
        get => GetValue(StatusPanelMarginProperty);
        set => SetValue(StatusPanelMarginProperty, value);
    }

    /// <summary>
    /// 是否使用相对时间模式显示时间轴
    /// </summary>
    public bool UseRelativeTime
    {
        get => GetValue(UseRelativeTimeProperty);
        set => SetValue(UseRelativeTimeProperty, value);
    }

    /// <summary>
    /// 相对时间的基准时间点（显示为 0s）
    /// </summary>
    public DateTime RelativeTimeBase
    {
        get => GetValue(RelativeTimeBaseProperty);
        set => SetValue(RelativeTimeBaseProperty, value);
    }

    /// <summary>
    /// 相对时间基准的显示标签（如"基准"、"动作时间"等）
    /// </summary>
    public string RelativeTimeBaseLabel
    {
        get => GetValue(RelativeTimeBaseLabelProperty);
        set => SetValue(RelativeTimeBaseLabelProperty, value);
    }

    /// <summary>
    /// 光标移动步长类型，控制按键移动的时间单位
    /// </summary>
    public CursorMoveStep CursorMoveStep
    {
        get => GetValue(CursorMoveStepProperty);
        set => SetValue(CursorMoveStepProperty, value);
    }

    /// <summary>
    /// 是否启用键盘控制光标移动
    /// </summary>
    public bool EnableKeyboardCursorControl
    {
        get => GetValue(EnableKeyboardCursorControlProperty);
        set => SetValue(EnableKeyboardCursorControlProperty, value);
    }

    public bool ShowCursor
    {
        get => GetValue(ShowCursorProperty);
        set => SetValue(ShowCursorProperty, value);
    }

    public bool ShowTimeAxis
    {
        get => GetValue(ShowTimeAxisProperty);
        set => SetValue(ShowTimeAxisProperty, value);
    }

    public double TimeAxisHeight
    {
        get => GetValue(TimeAxisHeightProperty);
        set => SetValue(TimeAxisHeightProperty, value);
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

    public bool IsInteractive
    {
        get => GetValue(IsInteractiveProperty);
        set => SetValue(IsInteractiveProperty, value);
    }

    public static readonly StyledProperty<double> ZoomLevelProperty =
        AvaloniaProperty.Register<WaveListPanel, double>(
            nameof(ZoomLevel), 1.0);

    public IChartGlobal ChartGlobal
    {
        get { return this; }
    }

    public double ZoomLevel
    {
        get => GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, value);
    }

    // 事件
    public static readonly RoutedEvent<TimeChangedEventArgs> CursorTimeChangedEvent =
        RoutedEvent.Register<WaveListPanel, TimeChangedEventArgs>(
            nameof(CursorTimeChanged), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<TimeRangeChangedEventArgs> TimeRangeChangedEvent =
        RoutedEvent.Register<WaveListPanel, TimeRangeChangedEventArgs>(
            nameof(TimeRangeChanged), RoutingStrategies.Bubble);

    public static readonly StyledProperty<double> LeftPaddingProperty =
        AvaloniaProperty.Register<WaveListPanel, double>(
            nameof(LeftPadding), 36);

    public static readonly StyledProperty<double> RightPaddingProperty =
        AvaloniaProperty.Register<WaveListPanel, double>(
            nameof(RightPadding), 40);

    /// <summary>
    /// 用于给Y轴坐标显示提供空间
    /// </summary>
    public double LeftPadding
    {
        get => GetValue(LeftPaddingProperty);
        set => SetValue(LeftPaddingProperty, value);
    }

    /// <summary>
    /// 用于给右侧显示提供空间
    /// </summary>
    public double RightPadding
    {
        get => GetValue(RightPaddingProperty);
        set => SetValue(RightPaddingProperty, value);
    }

    public double BaseInterval
    {
        get => _baseInterval;
    }

    public event EventHandler<TimeChangedEventArgs> CursorTimeChanged
    {
        add => AddHandler(CursorTimeChangedEvent, value);
        remove => RemoveHandler(CursorTimeChangedEvent, value);
    }

    public event EventHandler<TimeRangeChangedEventArgs> TimeRangeChanged
    {
        add => AddHandler(TimeRangeChangedEvent, value);
        remove => RemoveHandler(TimeRangeChangedEvent, value);
    }

    // 命令
    private readonly ICommand _zoomInCommand;
    public ICommand ZoomInCommand => _zoomInCommand;

    private readonly ICommand _zoomOutCommand;
    public ICommand ZoomOutCommand => _zoomOutCommand;

    // 控件部件
    private ScrollViewer? _scrollViewer;
    private Grid? _mainGrid;
    private TimeAxis? _timeAxisCanvas;
    private CursorCanvas? _cursorCanvas;
    private ItemsPresenter? _panelsStack;

    public event Action InvalidateRequestedEvent;
    public event Action ResetYViewRequestedEvent;

    private bool _isDragging = false;
    private double _lastPointerX = 0;

    public WaveListPanel()
    {
        _zoomInCommand = ReactiveCommand.Create(ZoomIn);
        _zoomOutCommand = ReactiveCommand.Create(ZoomOut);

        // 允许控件获取键盘焦点
        Focusable = true;

        // 时间范围变化时触发 CursorCanvas 重绘
        this.GetObservable(StartTimeProperty).Subscribe(_ => _cursorCanvas?.InvalidateVisual());
        this.GetObservable(EndTimeProperty).Subscribe(_ => _cursorCanvas?.InvalidateVisual());

        // 自动分配面板高度触发
        this.GetObservable(AutoDistributePanelHeightProperty).Subscribe(_ => DistributePanelHeight());
        this.GetObservable(StatusPanelHeightProperty).Subscribe(_ => DistributePanelHeight());
        this.GetObservable(StatusPanelMarginProperty).Subscribe(_ => DistributePanelHeight());

        // 监听 ItemsSource 变化
        this.GetObservable(ItemsSourceProperty).Subscribe(_ =>
        {
            if (AutoDistributePanelHeight)
            {
                Dispatcher.UIThread.Post(() => DistributePanelHeight());
            }
        });

        // 注册 Ctrl+R 快捷键
        KeyBindings.Add(new KeyBinding
        {
            Gesture = KeyGesture.Parse("Ctrl+R"),
            Command = ReactiveCommand.Create(ResetAllYViews)
        });
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var result = base.ArrangeOverride(finalSize);
        DistributePanelHeight();
        return result;
    }

    // 缩放功能
    private void ZoomIn()
    {
        ZoomInAtCursor();
    }

    private void ZoomOut()
    {
        ZoomOutAtCursor();
    }

    private void ZoomIn(double mouseX)
    {
        ZoomAt(mouseX, 0.8, 1.2);
    }

    private void ZoomOut(double mouseX)
    {
        ZoomAt(mouseX, 1.25, 0.8);
    }

    private void ZoomAt(double mouseX, double zoomFactor, double zoomLevelFactor)
    {
        if (_scrollViewer == null) return;

        // 将鼠标X坐标转换为对应的时间
        var centerTime = XToTime(mouseX);

        var timeSpan = EndTime - StartTime;
        var newSpan = TimeSpan.FromTicks((long)(timeSpan.Ticks * zoomFactor));

        // 以鼠标位置为中心：保持鼠标对应的时间在时间范围中的比例不变
        var centerRatio = (centerTime - StartTime).Ticks / (double)timeSpan.Ticks;
        var centerOffset = TimeSpan.FromTicks((long)(newSpan.Ticks * centerRatio));

        StartTime = centerTime - centerOffset;
        EndTime = StartTime + newSpan;
        ZoomLevel *= zoomLevelFactor;

        OnTimeRangeChanged();
    }

    private void ZoomAtTime(DateTime centerTime, double zoomFactor, double zoomLevelFactor)
    {
        var timeSpan = EndTime - StartTime;
        var newSpan = TimeSpan.FromTicks((long)(timeSpan.Ticks * zoomFactor));

        // 以指定时间为中心：保持该时间在时间范围中的比例不变
        var centerRatio = (centerTime - StartTime).Ticks / (double)timeSpan.Ticks;
        var centerOffset = TimeSpan.FromTicks((long)(newSpan.Ticks * centerRatio));

        StartTime = centerTime - centerOffset;
        EndTime = StartTime + newSpan;
        ZoomLevel *= zoomLevelFactor;

        OnTimeRangeChanged();
    }

    private void ZoomInAtCursor()
    {
        if (!CursorTime.HasValue) return;

        var timeSpan = EndTime - StartTime;
        var newSpan = timeSpan * 0.8; // 放大20%

        // 以光标为中心：保持光标在时间范围中的比例不变
        var cursorRatio = (CursorTime.Value - StartTime).Ticks / (double)timeSpan.Ticks;
        var cursorOffset = TimeSpan.FromTicks((long)(newSpan.Ticks * cursorRatio));

        StartTime = CursorTime.Value - cursorOffset;
        EndTime = StartTime + newSpan;
        ZoomLevel *= 1.2;

        OnTimeRangeChanged();
    }

    private void ZoomOutAtCursor()
    {
        if (!CursorTime.HasValue) return;

        var timeSpan = EndTime - StartTime;
        var newSpan = timeSpan * 1.25; // 缩小到80%

        // 以光标为中心：保持光标在时间范围中的比例不变
        var cursorRatio = (CursorTime.Value - StartTime).Ticks / (double)timeSpan.Ticks;
        var cursorOffset = TimeSpan.FromTicks((long)(newSpan.Ticks * cursorRatio));

        StartTime = CursorTime.Value - cursorOffset;
        EndTime = StartTime + newSpan;
        ZoomLevel *= 0.8;

        OnTimeRangeChanged();
    }

    #region 时间跳转功能

    /// <summary>
    /// 标志位，用于防止事件循环
    /// </summary>
    private bool _isJumping = false;

    /// <summary>
    /// 跳转到指定时间，保持当前时间跨度不变
    /// </summary>
    /// <param name="targetTime">目标中心时间</param>
    public void JumpToTime(DateTime targetTime)
    {
        if (_isJumping) return;

        try
        {
            _isJumping = true;
            var timeSpan = EndTime - StartTime;

            // 以目标时间为中心，保持时间跨度不变
            StartTime = targetTime - TimeSpan.FromTicks(timeSpan.Ticks / 2);
            EndTime = targetTime + TimeSpan.FromTicks(timeSpan.Ticks / 2);

            // 将光标移动到目标时间位置（内容区域中心）
            var viewportWidth = GetEffectiveViewportWidth();
            if (viewportWidth > LeftPadding + RightPadding)
            {
                // 光标位置 = LeftPadding + 内容区域宽度的一半
                CursorPosition = LeftPadding + (viewportWidth - LeftPadding - RightPadding) / 2;
            }

            OnTimeRangeChanged();
        }
        finally
        {
            _isJumping = false;
        }
    }

    /// <summary>
    /// 跳转到数据起始位置
    /// </summary>
    /// <param name="dataStartTime">数据起始时间</param>
    public void JumpToStart(DateTime dataStartTime)
    {
        var timeSpan = EndTime - StartTime;
        StartTime = dataStartTime;
        EndTime = dataStartTime + timeSpan;
        
        // 将光标移动到起始时间位置
        CursorPosition = LeftPadding;
        
        OnTimeRangeChanged();
    }

    /// <summary>
    /// 跳转到数据结束位置
    /// </summary>
    /// <param name="dataEndTime">数据结束时间</param>
    public void JumpToEnd(DateTime dataEndTime)
    {
        var timeSpan = EndTime - StartTime;
        EndTime = dataEndTime;
        StartTime = dataEndTime - timeSpan;
        
        // 将光标移动到结束时间位置
        var viewportWidth = GetEffectiveViewportWidth();
        if (viewportWidth > RightPadding)
        {
            CursorPosition = viewportWidth - RightPadding;
        }
        
        OnTimeRangeChanged();
    }

    /// <summary>
    /// 跳转到数据中间位置
    /// </summary>
    /// <param name="dataStartTime">数据起始时间</param>
    /// <param name="dataEndTime">数据结束时间</param>
    public void JumpToMiddle(DateTime dataStartTime, DateTime dataEndTime)
    {
        var middleTime = dataStartTime + TimeSpan.FromTicks((dataEndTime - dataStartTime).Ticks / 2);
        JumpToTime(middleTime);
    }

    /// <summary>
    /// 跳转到指定时间，并指定可见时间范围
    /// </summary>
    /// <param name="targetTime">目标中心时间</param>
    /// <param name="visibleSpan">可见时间跨度</param>
    public void JumpToTime(DateTime targetTime, TimeSpan visibleSpan)
    {
        if (_isJumping) return;

        try
        {
            _isJumping = true;
            StartTime = targetTime - TimeSpan.FromTicks(visibleSpan.Ticks / 2);
            EndTime = targetTime + TimeSpan.FromTicks(visibleSpan.Ticks / 2);
            
            // 将光标移动到目标时间位置（内容区域中心）
            var viewportWidth = GetEffectiveViewportWidth();
            if (viewportWidth > LeftPadding + RightPadding)
            {
                // 光标位置 = LeftPadding + 内容区域宽度的一半
                CursorPosition = LeftPadding + (viewportWidth - LeftPadding - RightPadding) / 2;
            }
            
            OnTimeRangeChanged();
        }
        finally
        {
            _isJumping = false;
        }
    }

    /// <summary>
    /// 获取有效的视口宽度
    /// </summary>
    private double GetEffectiveViewportWidth()
    {
        if (_scrollViewer != null && _scrollViewer.Viewport.Width > 0)
        {
            return _scrollViewer.Viewport.Width;
        }
        return Bounds.Width;
    }

    #endregion

    private void OnTimeRangeChanged()
    {
        UpdateCursorTime();
        _timeAxisCanvas?.InvalidateVisual();
        _scrollViewer?.InvalidateVisual();
        InvalidateRequestedEvent?.Invoke();
        var args = new TimeRangeChangedEventArgs(TimeRangeChangedEvent, StartTime, EndTime);

        RaiseEvent(args);
    }

    private void ResetAllYViews()
    {
        ResetYViewRequestedEvent?.Invoke();
    }

    // 应用模板
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
        _mainGrid = e.NameScope.Find<Grid>("PART_MainGrid");
        _timeAxisCanvas = e.NameScope.Find<TimeAxis>("PART_TimeAxisCanvas");
        _cursorCanvas = e.NameScope.Find<CursorCanvas>("PART_CursorCanvas");
        _panelsStack = e.NameScope.Find<ItemsPresenter>("ItemsPresenter");


        if (_scrollViewer != null)
        {
            _scrollViewer.PointerMoved += OnPointerMoved;
            _scrollViewer.PointerPressed += OnPointerPressed;
            _scrollViewer.PointerReleased += OnPointerReleased;
            if (_scrollViewer.Viewport.Width > LeftPadding + RightPadding)
            {
                CursorPosition = _scrollViewer.Viewport.Width / 2;
                UpdateCursorTime();
            }
        }

        // 在 MainGrid 上注册隧道事件，优先于 ScrollViewer 处理滚轮
        if (_mainGrid != null)
        {
            _mainGrid.AddHandler(PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
        }

        if (_timeAxisCanvas != null)
        {
            _timeAxisCanvas.MouseWheelZoom += OnTimeAxisMouseWheelZoom;
        }

        // 添加 Loaded 事件监听，在布局完成后分配面板高度
        Loaded += OnWaveListPanelLoaded;
    }

    private void OnWaveListPanelLoaded(object? sender, RoutedEventArgs e)
    {
        // 延迟执行，确保所有子元素布局完成
        Dispatcher.UIThread.Post(() =>
        {
            if (AutoDistributePanelHeight)
            {
                System.Diagnostics.Debug.WriteLine($"[DistributePanelHeight] Loaded触发 - ScrollViewer.Bounds.Height={_scrollViewer?.Bounds.Height}, MainGrid.Bounds.Height={_mainGrid?.Bounds.Height}, Bounds.Height={Bounds.Height}");
                DistributePanelHeight();
            }
        }, DispatcherPriority.Background);
    }

    private void OnTimeAxisMouseWheelZoom(object sender, MouseWheelZoomEventArgs e)
    {
        if (!IsInteractive) return;

        var centerTime = e.Time;

        if (e.Delta > 0)
        {
            ZoomAtTime(centerTime, 0.8, 1.2);
        }
        else
        {
            ZoomAtTime(centerTime, 1.25, 0.8);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (!EnableKeyboardCursorControl || _scrollViewer == null) return;

        var viewportWidth = _scrollViewer.Viewport.Width - LeftPadding - RightPadding;
        if (viewportWidth <= 0) return;

        var timeSpan = EndTime - StartTime;
        if (timeSpan.Ticks <= 0) return;

        if (e.Key == Key.Left || e.Key == Key.Right)
        {
            if (CursorTime.HasValue)
            {
                // 获取基础步长
                var step = GetCursorMoveStepTimeSpan();
                
                // Alt 键按下时，步长乘以 10
                if (e.KeyModifiers.HasFlag(KeyModifiers.Alt))
                {
                    step = TimeSpan.FromTicks(step.Ticks * 10);
                }

                // 根据方向移动
                if (e.Key == Key.Left)
                {
                    CursorTime = CursorTime.Value.Subtract(step);
                }
                else
                {
                    CursorTime = CursorTime.Value.Add(step);
                }

                CursorPosition = TimeToX(CursorTime.Value);
                
                // 标记事件已处理，阻止冒泡到 TabControl
                e.Handled = true;
            }
        }

        // 边界限制
        if (CursorPosition < LeftPadding)
        {
            CursorPosition = LeftPadding;
            CursorTime = XToTime(CursorPosition);
        }
        if (CursorPosition > _scrollViewer.Viewport.Width - RightPadding)
        {
            CursorPosition = _scrollViewer.Viewport.Width - RightPadding;
            CursorTime = XToTime(CursorPosition);
        }
    }

    /// <summary>
    /// 根据 CursorMoveStep 获取对应的时间跨度
    /// </summary>
    private TimeSpan GetCursorMoveStepTimeSpan()
    {
        return CursorMoveStep switch
        {
            CursorMoveStep.Microsecond => TimeSpan.FromTicks(10), // 1μs = 10 ticks
            CursorMoveStep.Millisecond => TimeSpan.FromMilliseconds(1),
            CursorMoveStep.Second => TimeSpan.FromSeconds(1),
            CursorMoveStep.Minute => TimeSpan.FromMinutes(1),
            CursorMoveStep.Hour => TimeSpan.FromHours(1),
            CursorMoveStep.Day => TimeSpan.FromDays(1),
            _ => TimeSpan.FromSeconds(1)
        };
    }

    // 鼠标事件处理
    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (!IsInteractive || _scrollViewer == null) return;

        if (_isDragging)
        {
            var position = e.GetCurrentPoint(_scrollViewer).Position;
            var deltaX = position.X - _lastPointerX;

            if (Math.Abs(deltaX) > 0.1)
            {
                var viewportWidth = _scrollViewer.Viewport.Width - LeftPadding - RightPadding;
                if (viewportWidth > 0)
                {
                    var timeSpan = EndTime - StartTime;
                    var timeDelta = TimeSpan.FromTicks((long)(timeSpan.Ticks * deltaX / viewportWidth));

                    StartTime -= timeDelta;
                    EndTime -= timeDelta;

                    OnTimeRangeChanged();
                }
            }

            _lastPointerX = position.X;
        }
    }

    private long _baseInterval;

    public List<long> CalculateTickIntervals()
    {
        var intervals = new List<long>();
        var visibleDuration = (EndTime - StartTime).Ticks;
        var pixelsPerTick = (Bounds.Width - LeftPadding - RightPadding) / (double)visibleDuration;

        // 根据缩放级别和像素密度计算合适的刻度间隔
        var baseInterval = CalculateBaseTickInterval(visibleDuration, pixelsPerTick);
        _baseInterval = baseInterval;
        // 生成刻度位置
        var firstTick = CalculateFirstTick(baseInterval);
        for (long tick = firstTick; tick <= EndTime.Ticks; tick += baseInterval)
        {
            intervals.Add(tick);
        }

        return intervals;
    }

    public double GetPixelsPerTick()
    {
        return (Bounds.Width - LeftPadding - RightPadding) / (double)(EndTime.Ticks - StartTime.Ticks);
    }


    private long CalculateFirstTick(long interval)
    {
        // 相对时间模式：刻度对齐到基准时间
        var alignmentBase = UseRelativeTime ? RelativeTimeBase.Ticks : 0;
        
        // 计算第一个刻度位置
        long firstTick;
        if (UseRelativeTime && alignmentBase != 0)
        {
            // 相对时间模式：刻度对齐到基准时间
            var remainder = (StartTime.Ticks - alignmentBase) % interval;
            firstTick = StartTime.Ticks - remainder;
            if (remainder < 0)
            {
                firstTick -= interval;
            }
            if (firstTick < StartTime.Ticks)
            {
                firstTick += interval;
            }
        }
        else
        {
            // 普通模式：对齐到自然时间
            var remainder = StartTime.Ticks % interval;
            firstTick = StartTime.Ticks - remainder + interval;
            /*if (remainder > interval / 2)
            {
                firstTick += interval;
            }*/
        }

        return firstTick;
    }

    private long CalculateBaseTickInterval(long visibleDuration, double pixelsPerTick)
    {
        // 预定义的刻度间隔（以ticks为单位）
        // 1 tick = 100 nanoseconds


        // 考虑缩放级别
        double effectivePixelsPerTick = pixelsPerTick;

        // 目标：每个刻度之间大约有 50-150 像素
        double targetPixelsPerTick = 80.0;
        double targetTickInterval = targetPixelsPerTick / effectivePixelsPerTick;

        // 找到最接近的预定义间隔
        long bestInterval = TimeAxisConsts.TimeIntervals[0];
        foreach (var interval in TimeAxisConsts.TimeIntervals)
        {
            if (interval >= targetTickInterval)
            {
                bestInterval = interval;
                break;
            }
        }

        // 如果所有间隔都太小，使用最大间隔的倍数
        if (bestInterval < targetTickInterval)
        {
            var maxInterval = TimeAxisConsts.TimeIntervals[TimeAxisConsts.TimeIntervals.Length - 1];
            bestInterval = maxInterval * (long)Math.Ceiling(targetTickInterval / maxInterval);
        }

        return bestInterval;
    }


    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (!IsInteractive) return;

        // 获取键盘焦点
        Focus();

        var position = e.GetCurrentPoint(_scrollViewer).Position;
        _isDragging = true;
        _lastPointerX = position.X;

        CursorPosition = position.X;

        UpdateCursorTime();

        return;
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
    }

    private void OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
    {
        // 首先阻止 ScrollViewer 的滚轮滚动
        e.Handled = true;
        
        if (!IsInteractive || _scrollViewer == null) return;

        // 检查是否按住 Ctrl 键进行 Y 轴缩放
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            // Y 轴缩放：找到鼠标下的 CurvePanel 并进行缩放
            var panel = FindCurvePanelAtPosition(e);
            if (panel != null)
            {
                double scaleFactor = e.Delta.Y > 0 ? 0.9 : 1.1;
               if (panel is CurvePanel curvePanel)
                {
                    curvePanel.ApplyYScale(scaleFactor);
                }
            }
            return;
        }

        // X 轴缩放
        var xPosition = e.GetCurrentPoint(_scrollViewer).Position;
        var delta = e.Delta.Y;
        if (delta > 0)
        {
            ZoomIn(xPosition.X);
        }
        else
        {
            ZoomOut(xPosition.X);
        }
    }

    private object? FindCurvePanelAtPosition(PointerWheelEventArgs e)
    {
        if (_scrollViewer?.Content is ItemsPresenter itemsPresenter)
        {
            itemsPresenter.ApplyTemplate();
            var panel = itemsPresenter.Panel;
            if (panel != null)
            {
                foreach (var child in panel.Children)
                {
                    Control? targetControl = null;
                    if (child is ContentPresenter contentPresenter)
                    {
                        targetControl = contentPresenter.Child;
                    }
                    else if (child is Control control)
                    {
                        targetControl = control;
                    }
                    if (targetControl is CurvePanel curvePanel)
                    {
                        var relativePos = e.GetCurrentPoint(curvePanel).Position;
                        if (relativePos.Y >= 0 && relativePos.Y <= curvePanel.Bounds.Height)
                        {
                            return curvePanel;
                        }
                    }
                }
            }
        }
        return null;
    }

    // 更新时间游标
    private void UpdateCursorTime()
    {
        if (!ShowCursor) return;

        var viewportWidth = GetEffectiveViewportWidth();
        
        if (CursorPosition < LeftPadding)
        {
            CursorPosition = LeftPadding;
        }

        if (viewportWidth > RightPadding && CursorPosition > viewportWidth - RightPadding)
        {
            CursorPosition = viewportWidth - RightPadding;
        }

        CursorTime = XToTime(CursorPosition);
    }


    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (_scrollViewer == null)
        {
            return;
        }
        if (_scrollViewer.Viewport.Width > LeftPadding + RightPadding)
        {
            CursorPosition = _scrollViewer.Viewport.Width / 2;
            UpdateCursorTime();
        }
    }


    public double TimeToX(DateTime time)
    {
        var viewportWidth = GetEffectiveViewportWidth() - LeftPadding - RightPadding;
        var timeSpan = EndTime - StartTime;
        if (timeSpan.Ticks <= 0 || viewportWidth <= 0) return double.NaN;

        var ratio = (time - StartTime).Ticks / (double)timeSpan.Ticks;
        var x = LeftPadding + viewportWidth * ratio;
        return x;
    }

    public DateTime XToTime(double x)
    {
        var viewportWidth = GetEffectiveViewportWidth() - LeftPadding - RightPadding;
        if (viewportWidth <= 0) return DateTime.MinValue;

        // 鼠标相对于内容区的位置（StartTime 对应的 X 位置是 LeftPadding）
        var relativeX = x - LeftPadding;

        var timeSpan = EndTime - StartTime;
        if (timeSpan.Ticks <= 0) return DateTime.MinValue;

        var ratio = Math.Max(0, Math.Min(1, relativeX / viewportWidth));
        var newTime = StartTime + TimeSpan.FromTicks((long)(timeSpan.Ticks * ratio));

        return newTime;
    }

    public DateTime GetVisibleEndTime()
    {
        return EndTime;
    }

    public DateTime GetVisibleStartTime()
    {
        return StartTime;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        // TimeMarker 和 TimeRangeMarker 的绘制已移至 CursorCanvas 控件
    }

    /// <summary>
    /// 自动分配面板高度：所有 CurvePanel 平均占满除 StatusPanel 以外的空间
    /// </summary>
    public void DistributePanelHeight()
    {
        if (!AutoDistributePanelHeight) return;
        if (_scrollViewer == null || Items == null) return;

        // 获取可用高度 - 优先使用 ScrollViewer 的实际渲染高度
        double availableHeight = _scrollViewer.Bounds.Height;
        
        // 如果 ScrollViewer 高度无效（可能是初始化阶段），尝试使用父容器
        if (availableHeight <= 0 && _mainGrid != null)
        {
            availableHeight = _mainGrid.Bounds.Height - 40;
        }
        
        // 最后尝试使用控件自身高度
        if (availableHeight <= 0)
        {
            availableHeight = Bounds.Height - 40;
        }
        
        if (availableHeight <= 0)
        {
            System.Diagnostics.Debug.WriteLine($"[DistributePanelHeight] 可用高度无效: {availableHeight}");
            return;
        }

        // 统计 CurvePanel 和 StatusPanel 数量（只计算可见的）
        int curvePanelCount = 0;
        int statusPanelCount = 0;

        foreach (var item in Items)
        {
            if (item is DataSeriesGroupBase group && !group.IsVisible)
                continue;
                
            if (item is CurveGroup)
                curvePanelCount++;
            else if (item is StatuSeriesGroup)
                statusPanelCount++;
        }

        if (curvePanelCount == 0)
        {
            System.Diagnostics.Debug.WriteLine($"[DistributePanelHeight] 没有 CurvePanel");
            return;
        }

        // 计算 StatusPanel 占用的总高度（包含 Margin）
        double statusPanelTotalHeight = statusPanelCount * (StatusPanelHeight + StatusPanelMargin.Top + StatusPanelMargin.Bottom);

        // 计算 CurvePanel 可用的总高度
        double curveAvailableHeight = availableHeight - statusPanelTotalHeight;
        if (curveAvailableHeight <= 0)
        {
            System.Diagnostics.Debug.WriteLine($"[DistributePanelHeight] CurvePanel 可用高度不足: {curveAvailableHeight}");
            return;
        }

        // 每个 CurvePanel 的高度
        double curvePanelHeight = curveAvailableHeight / curvePanelCount;

        System.Diagnostics.Debug.WriteLine($"[DistributePanelHeight] 计算结果: 可用高度={availableHeight}, CurvePanel数={curvePanelCount}, StatusPanel数={statusPanelCount}, 每个CurvePanel高度={curvePanelHeight}");

        // 更新所有面板的高度
        UpdatePanelHeights(curvePanelHeight);
    }

    /// <summary>
    /// 更新所有面板的高度
    /// </summary>
    private void UpdatePanelHeights(double curvePanelHeight)
    {
        if (_scrollViewer == null)
        {
            System.Diagnostics.Debug.WriteLine("[UpdatePanelHeights] _scrollViewer 为 null");
            return;
        }

        // 通过 ItemsPresenter 获取 Panel
        if (_scrollViewer.Content is ItemsPresenter itemsPresenter)
        {
            // 强制应用模板，确保 Panel 已创建
            itemsPresenter.ApplyTemplate();
            
            // Panel 可能是 VirtualizingStackPanel 或 StackPanel
            var panel = itemsPresenter.Panel;
            if (panel != null)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdatePanelHeights] Panel 类型: {panel.GetType().Name}, Children 数量: {panel.Children.Count}");
                
                int curveCount = 0, statusCount = 0;
                foreach (var child in panel.Children)
                {
                    // ItemsControl 的 Children 可能被包装在 ContentPresenter 中
                    Control? targetControl = null;
                    
                    if (child is ContentPresenter contentPresenter)
                    {
                        // 从 ContentPresenter 获取实际内容
                        targetControl = contentPresenter.Child;
                        System.Diagnostics.Debug.WriteLine($"[UpdatePanelHeights] ContentPresenter.Child 类型: {targetControl?.GetType().Name ?? "null"}");
                    }
                    else if (child is Control control)
                    {
                        targetControl = control;
                    }
                    
                   if (targetControl is CurvePanel curvePanel)
                    {
                        // 检查数据源是否可见
                        if (curvePanel.DataContext is DataSeriesGroupBase group && !group.IsVisible)
                        {
                            continue;
                        }
                        curvePanel.Height = curvePanelHeight;
                        curveCount++;
                    }
                    else if (targetControl is StatusPanel statusPanel)
                    {
                        // 检查数据源是否可见
                        if (statusPanel.DataContext is DataSeriesGroupBase group && !group.IsVisible)
                        {
                            continue;
                        }
                        statusPanel.Height = StatusPanelHeight;
                        statusCount++;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[UpdatePanelHeights] 未识别的子控件类型: {child.GetType().Name}");
                    }
                }
                System.Diagnostics.Debug.WriteLine($"[UpdatePanelHeights] 已更新 {curveCount} 个 CurvePanel, {statusCount} 个 StatusPanel");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[UpdatePanelHeights] Panel 为 null");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[UpdatePanelHeights] ScrollViewer.Content 不是 ItemsPresenter: {_scrollViewer.Content?.GetType().Name ?? "null"}");
        }
    }
}

// 事件参数类
public class TimeChangedEventArgs : RoutedEventArgs
{
    public DateTime Time { get; }

    public TimeChangedEventArgs(RoutedEvent routedEvent, DateTime time)
        : base(routedEvent)
    {
        Time = time;
    }
}

public class TimeRangeChangedEventArgs : RoutedEventArgs
{
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }

    public TimeRangeChangedEventArgs(RoutedEvent routedEvent, DateTime startTime, DateTime endTime)
        : base(routedEvent)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}
