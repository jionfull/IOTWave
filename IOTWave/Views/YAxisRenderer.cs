using Avalonia.Media;
using IotWave.Models;
using IotWave.Views;

namespace IOTChartBuddy.Controls
{
    public class YAxisRenderer
    {
        private readonly ChartPanelBase _panel;

        private double _yMin = 0;
        private double _yMax = 100;
        private double _yOffset = 0;
        private double _yScale = 1.0;
        private int _yTickCount = 5;
        private IList<YMarker> _yMarkers = new List<YMarker>();
        private Rect _bounds = default;

        public double YMin
        {
            get => _yMin;
            set => _yMin = value;
        }

        public double YMax
        {
            get => _yMax;
            set => _yMax = value;
        }

        public double YOffset
        {
            get => _yOffset;
            set => _yOffset = value;
        }

        public double YScale
        {
            get => _yScale;
            set => _yScale = value;
        }

        public int YTickCount
        {
            get => _yTickCount;
            set => _yTickCount = value;
        }

        public IList<YMarker> YMarkers
        {
            get => _yMarkers;
            set => _yMarkers = value ?? new List<YMarker>();
        }

        public Rect Bounds
        {
            get => _bounds;
            set => _bounds = value;
        }

        public IChartGlobal ChartGlobal;

        public YAxisRenderer(ChartPanelBase panel, IChartGlobal chartGlobal, Func<string, FormattedText> createFormattedText)
        {
            _panel = panel;

        }

        public void DrawYAxis(DrawingContext context)
        {
            if (_bounds.Width <= 0) return;

            double yAxisWidth = 50;
            var axisBrush = _panel.ChartGlobal.GridBrush ?? Brushes.Black;
            var axisPen = new Pen(axisBrush, _panel.ChartGlobal.GridThickness);

            context.DrawLine(axisPen,
                new Point(_panel.ChartGlobal.LeftPadding, 0),
                new Point(_panel.ChartGlobal.LeftPadding, _bounds.Height));

            double yRange = (_yMax - _yMin) / _yScale;
            double adjustedYMin = _yMin + _yOffset;
            double adjustedYMax = adjustedYMin + yRange;

            var baseInterval = CalculateBaseTickInterval(yRange);
            var firstTick = CalculateFirstTickValue(baseInterval, adjustedYMin);


            context.DrawLine(axisPen,
                new Point(0, _panel.Height),
                new Point(_bounds.Width - _panel.ChartGlobal.RightPadding, _panel.Height));
            var text = _panel.CreateFormattedText(adjustedYMin.ToString("F2"));
            double textWidth = text.Width;
            double textX = _panel.ChartGlobal.LeftPadding - textWidth - 2;
            double textY = _panel.Height - text.Height;
            context.DrawText(text, new Point(textX, textY));

            context.DrawLine(axisPen,
                new Point(0, 0),
                new Point(_bounds.Width - _panel.ChartGlobal.RightPadding, 0));
            text = _panel.CreateFormattedText(adjustedYMax.ToString("F2"));
            textWidth = text.Width;
            textX = _panel.ChartGlobal.LeftPadding - textWidth - 2;
            textY = 0;
            context.DrawText(text, new Point(textX, textY));

            for (double tickValue = firstTick; tickValue <= adjustedYMax - baseInterval * 0.5; tickValue += baseInterval)
            {
                double y = ValueToY(tickValue);

                context.DrawLine(axisPen,
                    new Point(_panel.ChartGlobal.LeftPadding, y),
                    new Point(_bounds.Width - _panel.ChartGlobal.RightPadding, y));

                text = _panel.CreateFormattedText(tickValue.ToString("F2"));
                textWidth = text.Width;
                textX = _panel.ChartGlobal.LeftPadding - textWidth - 2;

                if (textX < 0)
                {
                    textX = 2;
                }

                textY = y - text.Height / 2;

                if (textY < 0)
                {
                    textY = 2;
                }
                if (textY + text.Height > _bounds.Height)
                {
                    textY = _bounds.Height - text.Height - 2;
                }

                context.DrawText(text, new Point(textX, textY));
            }
        }

        public void DrawYMarkers(DrawingContext context)
        {
            double yAxisWidth = 50;

            foreach (var marker in _yMarkers)
            {
                double y = ValueToY(marker.Value);
                if (y >= 0 && y <= _bounds.Height)
                {
                    var pen = new Pen(Brushes.Red, 1);
                    context.DrawLine(pen, new Point(_panel.ChartGlobal.LeftPadding, y), new Point(_bounds.Width - _panel.ChartGlobal.RightPadding, y));

                    var text = _panel.CreateFormattedText(marker.Caption);

                    // 右侧与 EndTime 对齐（绘制区域右边界）
                    double textX = _bounds.Width - _panel.ChartGlobal.RightPadding - text.Width;

                    // 垂直方向：底部与 MarkerLine 对齐，留 1 像素空间（文本在曲线上方）
                    double textY = y  - 1;

                    // 边界检查
                    if (textY < 0)
                    {
                        textY = 0;
                    }

                    context.DrawText(text, new Point(textX, textY));
                }
            }
        }

        double exponent;
        private double CalculateBaseTickInterval(double yRange)
        {
            if (yRange <= 0) return 1.0;

            double valuePrePix = yRange / _panel.Height;
            double rawInterval = valuePrePix * 50;

            exponent = Math.Floor(Math.Log10(rawInterval));
            double fraction = rawInterval / Math.Pow(10, exponent);

            double niceFraction;
            if (fraction < 1.5)
                niceFraction = 1;
            else if (fraction < 3)
                niceFraction = 2;
            else if (fraction < 7)
                niceFraction = 5;
            else
                niceFraction = 10;

            return niceFraction * Math.Pow(10, exponent);
        }

        private double CalculateFirstTickValue(double baseInterval, double adjustedYMin)
        {
            var ret = Math.Ceiling(adjustedYMin / baseInterval) * baseInterval;

            if (ret - adjustedYMin < baseInterval / 2)
            {
                ret += baseInterval;
            }

            return ret;
        }

        public double ValueToY(double value)
        {
            double yRange = (_yMax - _yMin) / _yScale;
            double adjustedYMin = _yMin + _yOffset;
            double adjustedYMax = adjustedYMin + yRange;

            if (adjustedYMax - adjustedYMin == 0) return _bounds.Height / 2;

            double normalized = (value - adjustedYMin) / (adjustedYMax - adjustedYMin);
            return _bounds.Height * (1 - normalized);
        }
    }
}
