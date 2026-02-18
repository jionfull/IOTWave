using Avalonia.Controls.Primitives;
using IotWave.Views;

namespace IOTWave;

public partial class IOTChart : UserControl
{
    public static readonly StyledProperty<bool> AutoDistributePanelHeightProperty =
        AvaloniaProperty.Register<IOTChart, bool>(
            nameof(AutoDistributePanelHeight), true);

    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<IOTChart, ScrollBarVisibility>(
            nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Disabled);

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

    public IOTChart()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        
        // 将属性绑定到内部的 WaveListPanel
        if (this.FindControl<WaveListPanel>("CurveDisplay") is WaveListPanel waveListPanel)
        {
            waveListPanel.Bind(WaveListPanel.AutoDistributePanelHeightProperty,
                this.GetBindingObservable(AutoDistributePanelHeightProperty));
            waveListPanel.Bind(WaveListPanel.VerticalScrollBarVisibilityProperty,
                this.GetBindingObservable(VerticalScrollBarVisibilityProperty));
        }
    }
}
