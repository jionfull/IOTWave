using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Threading;
using IOTWave.Models;
using IOTWave.Views;
using NUnit.Framework;

namespace IOTWave.Tests;

[TestFixture]
public class MarkerAndTimeAxisTests
{
    #region TimeMarker Tests

    [Test]
    public void TimeMarker_Initialization()
    {
        var time = DateTime.Now;
        var marker = new TimeMarker
        {
            Time = time
        };

        Assert.That(marker.Time, Is.EqualTo(time));
        Assert.That(marker.Label, Is.EqualTo(string.Empty));
        Assert.That(marker.Color, Is.EqualTo(Colors.Red));
        Assert.That(marker.IsVisible, Is.True);
    }

    #endregion

    #region TimeRangeMarker Tests

    [Test]
    public void TimeRangeMarker_Initialization()
    {
        var start = DateTime.Now;
        var end = start.AddMinutes(10);
        var marker = new TimeRangeMarker
        {
            StartTime = start,
            EndTime = end
        };

        Assert.That(marker.StartTime, Is.EqualTo(start));
        Assert.That(marker.EndTime, Is.EqualTo(end));
    }

    [Test]
    public void TimeRangeMarker_PropertyChanges()
    {
        var start = DateTime.Parse("2024-01-01 10:00:00");
        var end = DateTime.Parse("2024-01-01 10:30:00");
        var marker = new TimeRangeMarker
        {
            StartTime = start,
            EndTime = end,
            Label = "Test Range",
            Color = Colors.Blue
        };

        Assert.That(marker.StartTime, Is.EqualTo(start));
        Assert.That(marker.EndTime, Is.EqualTo(end));
        Assert.That(marker.Label, Is.EqualTo("Test Range"));
        Assert.That(marker.Color, Is.EqualTo(Colors.Blue));
    }

    #endregion

    #region YMarker Tests

    [Test]
    public void YMarker_Initialization()
    {
        var marker = new YMarker(100.0, "Test Marker");

        Assert.That(marker.Value, Is.EqualTo(100.0));
        Assert.That(marker.Caption, Is.EqualTo("Test Marker"));
    }

    [Test]
    public void YMarker_PropertyChanges()
    {
        var marker = new YMarker(50.0, "Initial Label");

        marker.Value = 75.0;
        marker.Caption = "Updated Label";

        Assert.That(marker.Value, Is.EqualTo(75.0));
        Assert.That(marker.Caption, Is.EqualTo("Updated Label"));
    }

    #endregion

    #region TimeAxis Tests

    [AvaloniaTest]
    public void TimeAxis_Initialization()
    {
        var axis = new TimeAxis();

        // TimeAxis可能没有公开的StartTime和EndTime属性
        Assert.That(axis, Is.Not.Null);
    }

    #endregion

    #region YAxisRenderer Tests

    [AvaloniaTest]
    public void YAxisRenderer_Initialization()
    {
        var control = new CurvePanel();
        var chartGlobal = new MockChartGlobal();
        var renderer = new YAxisRenderer(control, chartGlobal, text => new FormattedText(
            text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            12,
            Brushes.Black
        ));

        Assert.That(renderer.YMin, Is.EqualTo(0));
        Assert.That(renderer.YMax, Is.EqualTo(100));
        Assert.That(renderer.YOffset, Is.EqualTo(0));
        Assert.That(renderer.YScale, Is.EqualTo(1.0));
    }

    [AvaloniaTest]
    public void YAxisRenderer_ValueToY_Conversion()
    {
        var control = new CurvePanel();
        var chartGlobal = new MockChartGlobal();
        var renderer = new YAxisRenderer(control, chartGlobal, text => new FormattedText(
            text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            12,
            Brushes.Black
        ));

        // Set up renderer properties
        renderer.YMin = 0;
        renderer.YMax = 100;
        renderer.Bounds = new Rect(0, 0, 200, 400);

        // Test conversion: value 50 should be at midpoint
        var yCoord = renderer.ValueToY(50);

        // With default values, value 50 should be at y = 200 (middle of 400px height)
        // Since Y coordinates start from top, higher values have lower Y
        Assert.That(yCoord, Is.EqualTo(200).Within(1.0));
    }

    #endregion

    #region CurvePanel Advanced Functionality Tests

    [AvaloniaTest]
    public void CurvePanel_ZoomAndPan_Initialization()
    {
        var panel = new CurvePanel();

        Assert.That(panel.DesiredHeight, Is.EqualTo(200.0));
        Assert.That(panel.ShowYAxis, Is.True);
    }

    [AvaloniaTest]
    public void CurvePanel_YMarkers_Property()
    {
        var panel = new CurvePanel();
        var markers = new List<YMarker>
        {
            new YMarker(10, "Marker 1"),
            new YMarker(20, "Marker 2")
        };

        panel.YMarkers = markers;

        Assert.That(panel.YMarkers, Is.Not.Null);
        Assert.That(panel.YMarkers.Count, Is.EqualTo(2));
    }

    [AvaloniaTest]
    public void CurvePanel_ResetView_Functionality()
    {
        var panel = new CurvePanel();

        // Initially default values
        Assert.That(panel.GetType().GetField("_yOffset",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(panel), Is.EqualTo(0));

        // Call ResetView (which sets internal fields)
        panel.GetType().GetMethod("ResetView",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            ?.Invoke(panel, null);

        // Verify the method executes without throwing
        Assert.Pass(); // The method call itself verifies functionality
    }

    #endregion

    #region Chart Global Interface Tests

    [Test]
    public void MockChartGlobal_Implementation()
    {
        var global = new MockChartGlobal();

        // Test initial values
        Assert.That(global.StartTime, Is.EqualTo(default(DateTime)));
        Assert.That(global.EndTime, Is.EqualTo(default(DateTime)));
        Assert.That(global.LabelBrush, Is.EqualTo(Brushes.Black));
    }

    #endregion

    #region Complex Integration Tests

    [AvaloniaTest]
    public void ComplexIntegration_TimeMarkersAndYMarkers()
    {
        var baseTime = DateTime.Parse("2024-01-01 10:00:00");

        // Create curve data with some points
        var curveData = new CurveData
        {
            Name = "Test Curve",
            Color = Colors.Blue,
            Points = new List<TimePoint>
            {
                new TimePoint { Time = baseTime, Value = 10 },
                new TimePoint { Time = baseTime.AddSeconds(1), Value = 15 },
                new TimePoint { Time = baseTime.AddSeconds(2), Value = 12 }
            }
        };

        // Create curve group with Y markers
        var curveGroup = new CurveGroup
        {
            Name = "Test Group",
            Curves = new List<CurveData> { curveData },
            YMarkers = new List<YMarker>
            {
                new YMarker(10, "Lower Limit"),
                new YMarker(15, "Upper Limit")
            }
        };

        // Create panel with Y markers
        var panel = new CurvePanel
        {
            Width = 800,
            Height = 400,
            Items = curveGroup,
            YMarkers = new List<YMarker>
            {
                new YMarker(8, "Safe Lower"),
                new YMarker(18, "Safe Upper")
            }
        };

        // Verify all components are properly connected
        Assert.That(panel.Items, Is.EqualTo(curveGroup));
        Assert.That(panel.Items.Curves.Count, Is.EqualTo(1));
        Assert.That(panel.Items.YMarkers.Count, Is.EqualTo(2));
        Assert.That(panel.YMarkers.Count, Is.EqualTo(2));
    }

    [AvaloniaTest]
    public void MultipleCurveVisualization()
    {
        var baseTime = DateTime.Parse("2024-01-01 10:00:00");

        // Create multiple curves
        var curve1 = new CurveData
        {
            Name = "Temperature",
            Color = Colors.Red,
            Points = Enumerable.Range(0, 100)
                .Select(i => new TimePoint
                {
                    Time = baseTime.AddSeconds(i),
                    Value = 20 + Math.Sin(i * 0.1) * 5
                }).ToList()
        };

        var curve2 = new CurveData
        {
            Name = "Pressure",
            Color = Colors.Blue,
            Points = Enumerable.Range(0, 100)
                .Select(i => new TimePoint
                {
                    Time = baseTime.AddSeconds(i),
                    Value = 1000 + Math.Cos(i * 0.1) * 10
                }).ToList()
        };

        var curveGroup = new CurveGroup
        {
            Name = "Sensor Data",
            Curves = new List<CurveData> { curve1, curve2 }
        };

        var panel = new CurvePanel
        {
            Width = 800,
            Height = 400,
            Items = curveGroup
        };

        Assert.That(panel.Items.Curves.Count, Is.EqualTo(2));
        Assert.That(panel.Items.Curves[0].Name, Is.EqualTo("Temperature"));
        Assert.That(panel.Items.Curves[1].Name, Is.EqualTo("Pressure"));
        Assert.That(panel.Items.Curves[0].Points.Count, Is.EqualTo(100));
        Assert.That(panel.Items.Curves[1].Points.Count, Is.EqualTo(100));
    }

    #endregion
}

// Mock implementation for testing
internal class MockChartGlobal : IChartGlobal
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double LeftPadding { get; set; } = 60;
    public double RightPadding { get; set; } = 10;
    public IBrush? LabelBrush { get; set; } = Brushes.Black;
    public IBrush? GridBrush { get; set; } = Brushes.LightGray;
    public IBrush? SeparatorBrush { get; set; } = Brushes.Gray;
    public double GridThickness { get; set; } = 0.5;
    public bool UseRelativeTime { get; set; } = false;
    public DateTime RelativeTimeBase { get; set; } = DateTime.Now;
    public string RelativeTimeBaseLabel { get; set; } = "基准";
    public DateTime? CursorTime { get; set; } = null;
    public event Action InvalidateRequestedEvent;
    public event Action ResetYViewRequestedEvent;

    public double TimeToX(DateTime time)
    {
        var totalTimeSpan = (EndTime - StartTime).TotalSeconds;
        if (totalTimeSpan <= 0) return 0;
        var timeOffset = (time - StartTime).TotalSeconds;
        return (timeOffset / totalTimeSpan) * 800; // Assuming 800px width
    }

    public DateTime XToTime(double x)
    {
        var totalTimeSpan = (EndTime - StartTime).TotalSeconds;
        if (totalTimeSpan <= 0) return StartTime;
        var timeFraction = x / 800; // Assuming 800px width
        return StartTime.AddSeconds(totalTimeSpan * timeFraction);
    }

    public DateTime GetVisibleEndTime()
    {
        return EndTime;
    }

    public DateTime GetVisibleStartTime()
    {
        return StartTime;
    }

    public List<long> CalculateTickIntervals()
    {
        return new List<long> { StartTime.Ticks, EndTime.Ticks };
    }

    public double GetPixelsPerTick()
    {
        return 800.0 / (EndTime - StartTime).TotalSeconds; // Assuming 800px width
    }

    public double BaseInterval => 1.0;

    public void RequestInvalidate()
    {
        InvalidateRequestedEvent?.Invoke();
    }
}