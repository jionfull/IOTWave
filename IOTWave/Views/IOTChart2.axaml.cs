using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

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

    public static readonly StyledProperty<CursorMoveStep> CursorMoveStepProperty =
        AvaloniaProperty.Register<IOTChart2, CursorMoveStep>(
            nameof(CursorMoveStep), CursorMoveStep.Second);

    public static readonly StyledProperty<bool> EnableKeyboardCursorControlProperty =
        AvaloniaProperty.Register<IOTChart2, bool>(
            nameof(EnableKeyboardCursorControl), true);

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

    /// <summary>
    /// 获取内部的 WaveListPanel 控件，用于直接调用跳转方法
    /// </summary>
    public WaveListPanel? WaveListPanel => _waveListPanel;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _waveListPanel = e.NameScope.Find<WaveListPanel>("CurveDisplay");
    }
}
