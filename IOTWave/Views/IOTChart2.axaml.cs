using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using IOTWave.ViewModels;

namespace IOTWave.Views;

public class IOTChart2 : TemplatedControl
{
    public static readonly StyledProperty<bool> AutoDistributePanelHeightProperty =
        AvaloniaProperty.Register<IOTChart2, bool>(
            nameof(AutoDistributePanelHeight), true);

    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<IOTChart2, ScrollBarVisibility>(
            nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Disabled);

    public static readonly StyledProperty<bool> UseRelativeTimeProperty =
        AvaloniaProperty.Register<IOTChart2, bool>(
            nameof(UseRelativeTime), false);

    public static readonly StyledProperty<DateTime> RelativeTimeBaseProperty =
        AvaloniaProperty.Register<IOTChart2, DateTime>(
            nameof(RelativeTimeBase), DateTime.Now);

    public static readonly StyledProperty<string> RelativeTimeBaseLabelProperty =
        AvaloniaProperty.Register<IOTChart2, string>(
            nameof(RelativeTimeBaseLabel), "基准");

    private WaveListPanel? _waveListPanel;

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _waveListPanel = e.NameScope.Find<WaveListPanel>("CurveDisplay");

        // 订阅 DataContext 变化
        this.GetObservable(DataContextProperty).Subscribe(OnDataContextChanged);
    }

    private IOTWaveBaseViewModel? _currentViewModel;

    private void OnDataContextChanged(object? dataContext)
    {
        // 取消旧 ViewModel 的订阅
        if (_currentViewModel != null)
        {
            _currentViewModel.TimeJumpRequested -= OnTimeJumpRequested;
        }

        _currentViewModel = dataContext as IOTWaveBaseViewModel;

        // 订阅新 ViewModel 的事件
        if (_currentViewModel != null)
        {
            _currentViewModel.TimeJumpRequested += OnTimeJumpRequested;
        }
    }

    private void OnTimeJumpRequested(TimeJumpEventArgs e)
    {
        if (_waveListPanel == null || _currentViewModel == null) return;

        switch (e.JumpType)
        {
            case TimeJumpType.Start:
                _waveListPanel.JumpToStart(_currentViewModel.DataStartTime);
                break;
            case TimeJumpType.End:
                _waveListPanel.JumpToEnd(_currentViewModel.DataEndTime);
                break;
            case TimeJumpType.Middle:
                _waveListPanel.JumpToMiddle(_currentViewModel.DataStartTime, _currentViewModel.DataEndTime);
                break;
            case TimeJumpType.SpecificTime:
                if (e.TargetTime.HasValue)
                {
                    _waveListPanel.JumpToTime(e.TargetTime.Value);
                }
                break;
        }
    }
}
