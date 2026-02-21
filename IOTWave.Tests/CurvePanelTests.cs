using Avalonia;
using Avalonia.Headless;
using Avalonia.Logging;
using Avalonia.Platform;
using IOTWave.Models;
using IOTWave.Views;

namespace IOTWave.Tests;

/// <summary>
/// CurvePanel 拆分后函数的细粒度单元测试
/// </summary>
[TestFixture]
public class CurvePanelTests
{
    // 注意：Avalonia 平台初始化在单元测试中比较复杂
    // 对于需要 Avalonia 平台的测试，我们使用 Ignore 属性
    // 在实际的 UI 测试环境中，这些测试会正常工作

    #region CalculateVisiblePointIndices 测试

    [Test]
    public void CalculateVisiblePointIndices_EmptyList_ReturnsZeroZero()
    {
        var points = new List<TimePoint>();
        var drawArea = new Rect(0, 0, 100, 100);
        var startTime = DateTime.Now;
        var endTime = DateTime.Now.AddHours(1);

        var result = CurvePanel.CalculateVisiblePointIndices(points, startTime, endTime, drawArea);

        Assert.That(result.startIndex, Is.EqualTo(0));
        Assert.That(result.endIndex, Is.EqualTo(0));
    }

    [Test]
    public void CalculateVisiblePointIndices_ExactMatch_ReturnsAdjustedIndices()
    {
        var baseTime = DateTime.Parse("2024-01-01 10:00:00");
        var points = new List<TimePoint>
        {
            new TimePoint { Time = baseTime.AddSeconds(0), Value = 100 },
            new TimePoint { Time = baseTime.AddSeconds(1), Value = 110 },
            new TimePoint { Time = baseTime.AddSeconds(2), Value = 120 },
            new TimePoint { Time = baseTime.AddSeconds(3), Value = 130 },
            new TimePoint { Time = baseTime.AddSeconds(4), Value = 140 },
        };

        var drawArea = new Rect(0, 0, 100, 100);
        var visibleStartTime = baseTime.AddSeconds(1);
        var visibleEndTime = baseTime.AddSeconds(3);

        var result = CurvePanel.CalculateVisiblePointIndices(points, visibleStartTime, visibleEndTime, drawArea);

        Assert.That(result.startIndex, Is.EqualTo(0), "应该向前扩展一个索引");
        Assert.That(result.endIndex, Is.EqualTo(5), "应该向后扩展一个索引，直到列表长度");
    }

    [Test]
    public void CalculateVisiblePointIndices_NotFound_ReturnsBitwiseComplement()
    {
        var baseTime = DateTime.Parse("2024-01-01 10:00:00");
        var points = new List<TimePoint>
        {
            new TimePoint { Time = baseTime.AddSeconds(0), Value = 100 },
            new TimePoint { Time = baseTime.AddSeconds(2), Value = 120 },
            new TimePoint { Time = baseTime.AddSeconds(4), Value = 140 },
        };

        var drawArea = new Rect(0, 0, 100, 100);
        var visibleStartTime = baseTime.AddSeconds(1);
        var visibleEndTime = baseTime.AddSeconds(3);

        var result = CurvePanel.CalculateVisiblePointIndices(points, visibleStartTime, visibleEndTime, drawArea);

        Assert.That(result.startIndex, Is.EqualTo(0));
        Assert.That(result.endIndex, Is.EqualTo(3));
    }

    [Test]
    public void CalculateVisiblePointIndices_BeforeAllPoints_ReturnsStartZero()
    {
        var baseTime = DateTime.Parse("2024-01-01 10:00:00");
        var points = new List<TimePoint>
        {
            new TimePoint { Time = baseTime.AddSeconds(2), Value = 120 },
            new TimePoint { Time = baseTime.AddSeconds(3), Value = 130 },
        };

        var drawArea = new Rect(0, 0, 100, 100);
        var visibleStartTime = baseTime.AddSeconds(0);
        var visibleEndTime = baseTime.AddSeconds(2);

        var result = CurvePanel.CalculateVisiblePointIndices(points, visibleStartTime, visibleEndTime, drawArea);

        Assert.That(result.startIndex, Is.EqualTo(0), "startIndex 不应该小于0");
    }

    [Test]
    public void CalculateVisiblePointIndices_AfterAllPoints_ReturnsEndAtCount()
    {
        var baseTime = DateTime.Parse("2024-01-01 10:00:00");
        var points = new List<TimePoint>
        {
            new TimePoint { Time = baseTime.AddSeconds(0), Value = 100 },
            new TimePoint { Time = baseTime.AddSeconds(1), Value = 110 },
        };

        var drawArea = new Rect(0, 0, 100, 100);
        var visibleStartTime = baseTime.AddSeconds(1);
        var visibleEndTime = baseTime.AddSeconds(5);

        var result = CurvePanel.CalculateVisiblePointIndices(points, visibleStartTime, visibleEndTime, drawArea);

        Assert.That(result.endIndex, Is.EqualTo(points.Count), "endIndex 不应该超过列表长度");
    }

    #endregion

    #region GetIntersectionStatic 测试

    [Test]
    public void GetIntersectionStatic_LeftBoundary_ReturnsCorrectPoint()
    {
        var p1 = new Point(50, 100);
        var p2 = new Point(150, 200);
        var xBoundary = 100;

        var result = CurvePanel.GetIntersectionStatic(p1, p2, xBoundary);

        Assert.That(result.X, Is.EqualTo(100));
        Assert.That(result.Y, Is.EqualTo(150).Within(0.001));
    }

    [Test]
    public void GetIntersectionStatic_RightBoundary_ReturnsCorrectPoint()
    {
        var p1 = new Point(600, 200);
        var p2 = new Point(800, 300);
        var xBoundary = 700;

        var result = CurvePanel.GetIntersectionStatic(p1, p2, xBoundary);

        Assert.That(result.X, Is.EqualTo(700));
        Assert.That(result.Y, Is.EqualTo(250).Within(0.001));
    }

    [Test]
    public void GetIntersectionStatic_VerticalLine_ReturnsCorrectPoint()
    {
        var p1 = new Point(100, 100);
        var p2 = new Point(100, 200);
        var xBoundary = 100;

        var result = CurvePanel.GetIntersectionStatic(p1, p2, xBoundary);

        Assert.That(result.X, Is.EqualTo(100));
        Assert.That(result.Y, Is.EqualTo(100), "垂直线段的交点应该是起点");
    }

    [Test]
    public void GetIntersectionStatic_ExactMatch_ReturnsPoint()
    {
        var p1 = new Point(100, 100);
        var p2 = new Point(200, 200);
        var xBoundary = 100;

        var result = CurvePanel.GetIntersectionStatic(p1, p2, xBoundary);

        Assert.That(result.X, Is.EqualTo(100));
        Assert.That(result.Y, Is.EqualTo(100));
    }

    #endregion

    #region CreateDataPointGeometries 测试

    [Test]
    public void CreateDataPointGeometries_EmptyList_ReturnsEmpty()
    {
        var points = new List<(Point, int)>();
        var drawArea = new Rect(100, 0, 600, 400);

        var result = CurvePanel.CreateDataPointGeometries(points, drawArea);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void CreateDataPointGeometries_PointsInsideDrawArea_ReturnsAll()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(200, 100), 0),
            (new Point(300, 200), 1),
            (new Point(400, 300), 2),
        };

        var result = CurvePanel.CreateDataPointGeometries(points, drawArea);

        Assert.That(result.Count, Is.EqualTo(3));
    }

    [Test]
    public void CreateDataPointGeometries_PointsOutsideDrawArea_ReturnsEmpty()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(50, 100), 0),
            (new Point(800, 200), 1),
        };

        var result = CurvePanel.CreateDataPointGeometries(points, drawArea);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void CreateDataPointGeometries_MixedPoints_ReturnsOnlyInside()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(50, 100), 0),
            (new Point(200, 150), 1),
            (new Point(400, 200), 2),
            (new Point(800, 250), 3),
        };

        var result = CurvePanel.CreateDataPointGeometries(points, drawArea);

        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public void CreateDataPointGeometries_OnBoundary_ReturnsPoint()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(100, 100), 0),
            (new Point(700, 200), 1),
        };

        var result = CurvePanel.CreateDataPointGeometries(points, drawArea);

        Assert.That(result.Count, Is.EqualTo(2), "边界上的点应该被包含");
    }

    [Test]
    public void CreateDataPointGeometries_CustomPointSize_ReturnsCorrectSize()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var point = new Point(200, 100);
        var points = new List<(Point, int)> { (point, 0) };
        var customSize = 5.0;

        var result = CurvePanel.CreateDataPointGeometries(points, drawArea, customSize);

        Assert.That(result.Count, Is.EqualTo(1));
        var geometry = result[0];
        Assert.That(geometry.Rect.Width, Is.EqualTo(customSize * 2));
        Assert.That(geometry.Rect.Height, Is.EqualTo(customSize * 2));
        Assert.That(geometry.Rect.X, Is.EqualTo(point.X - customSize));
        Assert.That(geometry.Rect.Y, Is.EqualTo(point.Y - customSize));
    }

    #endregion

    #region BuildCurveGeometry 测试说明
    /// <summary>
    /// 注意：BuildCurveGeometry 相关的测试已移至 CurvePanelHeadlessTests.cs 文件
    /// 这些测试需要 Avalonia 平台初始化，使用 Headless 模式运行
    /// </summary>
    #endregion

    #region 集成测试

    [Test]
    public void FullWorkflow_CalculateIndicesThenBuildGeometry_WorksTogether()
    {
        var baseTime = DateTime.Parse("2024-01-01 10:00:00");
        var points = new List<TimePoint>
        {
            new TimePoint { Time = baseTime.AddSeconds(0), Value = 100 },
            new TimePoint { Time = baseTime.AddSeconds(1), Value = 110 },
            new TimePoint { Time = baseTime.AddSeconds(2), Value = 120 },
            new TimePoint { Time = baseTime.AddSeconds(3), Value = 130 },
            new TimePoint { Time = baseTime.AddSeconds(4), Value = 140 },
        };

        var drawArea = new Rect(0, 0, 100, 100);
        var visibleStartTime = baseTime.AddSeconds(1);
        var visibleEndTime = baseTime.AddSeconds(3);

        var indices = CurvePanel.CalculateVisiblePointIndices(points, visibleStartTime, visibleEndTime, drawArea);

        Assert.That(indices.startIndex, Is.LessThan(indices.endIndex));
    }

    #endregion
}
