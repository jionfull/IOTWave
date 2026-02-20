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

    public static readonly StyledProperty<bool> ShowCurrentValueProperty =
        AvaloniaProperty.Register<IOTChart2, bool>(
            nameof(ShowCurrentValue), false);

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
    /// 是否在图例中显示光标位置的当前值
    /// </summary>
    public bool ShowCurrentValue
    {
        get => GetValue(ShowCurrentValueProperty);
        set => SetValue(ShowCurrentValueProperty, value);
    }
}
