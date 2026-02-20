using Avalonia.Controls.Primitives;

namespace IOTWave;

public partial class IOTChart : UserControl
{
    public static readonly StyledProperty<bool> AutoDistributePanelHeightProperty =
        AvaloniaProperty.Register<IOTChart, bool>(
            nameof(AutoDistributePanelHeight), true);

    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<IOTChart, ScrollBarVisibility>(
            nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Disabled);

    public static readonly StyledProperty<bool> UseRelativeTimeProperty =
        AvaloniaProperty.Register<IOTChart, bool>(
            nameof(UseRelativeTime), false);

    public static readonly StyledProperty<DateTime> RelativeTimeBaseProperty =
        AvaloniaProperty.Register<IOTChart, DateTime>(
            nameof(RelativeTimeBase), DateTime.Now);

    public static readonly StyledProperty<string> RelativeTimeBaseLabelProperty =
        AvaloniaProperty.Register<IOTChart, string>(
            nameof(RelativeTimeBaseLabel), "基准");

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

    public IOTChart()
    {
        InitializeComponent();
    }
}
