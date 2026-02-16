using Avalonia.Data;
using IotWave.Views;

namespace IOTChartBuddy.Controls
{
    /// <summary>
    /// 图表面板基类
    /// </summary>
    public abstract class ChartPanelBase : UserControl
    {
        public static readonly StyledProperty<IChartGlobal> ChartGlobalProperty =
        AvaloniaProperty.Register<ChartPanelBase, IChartGlobal>(nameof(ChartGlobal), null, true, BindingMode.OneWay);

        public IChartGlobal ChartGlobal
        {
            get => GetValue(ChartGlobalProperty);
            set => SetValue(ChartGlobalProperty, value);
        }

        private void OnInvalidateRequested()
        {
            InvalidateVisual();
        }

    public static readonly StyledProperty<IBrush?> PanelBackgroundProperty =
            AvaloniaProperty.Register<ChartPanelBase, IBrush?>(nameof(PanelBackground), Brushes.Transparent);
    
  

        public static readonly StyledProperty<double> PanelFontSizeProperty =
            AvaloniaProperty.Register<ChartPanelBase, double>(nameof(PanelFontSize), 12.0);

        public static readonly StyledProperty<FontFamily?> PanelFontFamilyProperty =
            AvaloniaProperty.Register<ChartPanelBase, FontFamily?>(nameof(PanelFontFamily), FontFamily.Default);

        public IBrush? PanelBackground
        {
            get => GetValue(PanelBackgroundProperty);
            set => SetValue(PanelBackgroundProperty, value);
        }

     
        public double PanelFontSize
        {
            get => GetValue(PanelFontSizeProperty);
            set => SetValue(PanelFontSizeProperty, value);
        }

        public FontFamily? PanelFontFamily
        {
            get => GetValue(PanelFontFamilyProperty);
            set => SetValue(PanelFontFamilyProperty, value);
        }

        protected ChartPanelBase()
        {
            ClipToBounds = true;
            this.GetObservable(ChartGlobalProperty).Subscribe(OnChartGlobalChanged);
        }

        private void OnChartGlobalChanged(IChartGlobal? oldGlobal)
        {
            if (oldGlobal != null)
            {
                oldGlobal.InvalidateRequestedEvent -= OnInvalidateRequested;
            }

            if (ChartGlobal != null)
            {
                ChartGlobal.InvalidateRequestedEvent += OnInvalidateRequested;
            }
        }

        protected double TimeToX(DateTime time)
        {
            return ChartGlobal?.TimeToX(time) ?? 0;
        }

        protected DateTime XToTime(double x)
        {
            return ChartGlobal?.XToTime(x) ?? DateTime.Now;
        }

   

        protected DateTime GetVisibleEndTime()
        {
            return ChartGlobal?.GetVisibleEndTime() ?? DateTime.Now;
        }

      
        protected DateTime GetVisibleStartTime()
        {
            return ChartGlobal?.GetVisibleStartTime()??DateTime.Now;
        }


        public FormattedText CreateFormattedText(string text)
        {
            var typeface = new Typeface(PanelFontFamily ?? FontFamily.Default);
            var labelBrush = ChartGlobal?.LabelBrush ?? Brushes.Black;
            return new FormattedText(text, 
                System.Globalization.CultureInfo.CurrentCulture, 
                FlowDirection.LeftToRight, 
                typeface, PanelFontSize, labelBrush);
        }
    }
}